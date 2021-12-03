using System;
using System.Text;
using AstmLib.DataLinkLayer.Exceptions;
using AstmLib.Configuration;
using AstmLib.Utilities;
using Microsoft.Extensions.Logging;

namespace AstmLib.DataLinkLayer
{
    public interface IUploader
    {
        bool Completed { get; }
        string Message { get; }
        void ResetAndUpload(string message);

        /// <summary>
        /// Upload one frame and returns control. Keeps its state and waits for the next call to complete upload.
        /// </summary>
        /// <exception cref="UploadException">Any exception during upload</exception>
        /// <exception cref="InvalidOperationException">Upload already done</exception>
        void ExecuteUploadStep();
    }

    public class Uploader : IUploader
    {
        private readonly ILogger<Uploader> _log;

        private enum States
        {
            EstablishmentPhaseBegin,
            EstablishmentPhaseWaitAnswer,
            TransferPhaseSendFrame,
            TransferPhaseResendFrame,
            TransferPhaseWaitAnswer,
            TerminationPhase,
        }

        private readonly ITimersManager _timersManager;
        private const string WAIT_ANSWER_TIMER_NAME = "timer1";
        private const string WAIT_DELAY_TIMER_NAME = "timer2";

        private string _message;
        private readonly IAstmChannel _stream;
        private Frame[] _frames;
        private int _frameCounter = -1;
        private DataLinkLayerException _exceptionOccured = null;
        private int _errorCounterNAK = 0;
        private int _errorCounterENQ = 0;
        private int _errorCounterDefective = 0;
        private readonly bool _havePriority = true;
        private Frame _currentFrame = null;
        private bool _uploadCompleted = false;
        private States _state;

        public bool Completed => _uploadCompleted;

        public string Message => _message;

        private readonly AstmLowLevelSettings _lowLevelSettings;

        #region Constructor

        public Uploader(IAstmChannel stream, AstmLowLevelSettings lowLevelSettings, ITimersManager timersManager, ILogger<Uploader> log)
        {
            _stream = stream;
            _timersManager = timersManager;
            _log = log;
            _timersManager.CreateTimer(WAIT_ANSWER_TIMER_NAME);
            _timersManager.CreateTimer(WAIT_DELAY_TIMER_NAME);
            _havePriority = lowLevelSettings.HavePriority;
            _lowLevelSettings = lowLevelSettings;
            _timersManager = timersManager;
        }

        #endregion

        public void ResetAndUpload(string message)
        {
            _timersManager.StopAllTimers();
            _frameCounter = -1;
            _exceptionOccured = null;
            _errorCounterNAK = 0;
            _errorCounterENQ = 0;
            _errorCounterDefective = 0;
            _currentFrame = null;
            _uploadCompleted = false;
            _state = States.EstablishmentPhaseBegin;
            _message = message;
            _frames = FrameUtils.SplitToFrames(message, _lowLevelSettings);
        }

        /// <summary>
        /// Upload one frame and returns control. Keeps its state and waits for the next call to complete upload.
        /// </summary>
        /// <exception cref="UploadException">Any exception during upload</exception>
        /// <exception cref="InvalidOperationException">Upload already done</exception>
        public void ExecuteUploadStep()
        {
            byte[] buf = null;

            if (Completed)
            {
                throw new InvalidOperationException("Upload has been completed already");
            }

            if (!_timersManager.CheckTimerTimeout(WAIT_DELAY_TIMER_NAME))
            {
                return;
            }

            switch (_state)
            {
                case States.EstablishmentPhaseBegin:
                    _stream.WriteByte((byte) DataLinkControlCodes.ENQ);
                    _log.LogInformation("[send]{0}", ControlCodesUtility.ToControlCode((byte) DataLinkControlCodes.ENQ));
                    _timersManager.StartTimer(WAIT_ANSWER_TIMER_NAME, _lowLevelSettings.EnqWaitTimeout);
                    _state = States.EstablishmentPhaseWaitAnswer;
                    break;
                case States.EstablishmentPhaseWaitAnswer:
                    try
                    {
                        _stream.ReadTimeout = 100;
                        var b = (byte) _stream.ReadByte();
                        _timersManager.StopTimer(WAIT_ANSWER_TIMER_NAME);
                        _log.LogInformation("[receive]{0}", ControlCodesUtility.ToControlCode(b));
                        if (b == (byte) DataLinkControlCodes.ACK)
                        {
                            _state = States.TransferPhaseSendFrame;
                        }
                        else if (b == (byte) DataLinkControlCodes.ENQ)
                        {
                            if (_havePriority)
                            {
                                _errorCounterENQ++;
                                if (_errorCounterENQ >= 2)
                                {
                                    throw new DataLinkLayerException(new UnexpectedENQException(), "Remote system is stupid cow");
                                }

                                _timersManager.StartTimer(WAIT_DELAY_TIMER_NAME, 1000);
                                _state = States.EstablishmentPhaseBegin;
                            }
                            else
                            {
                                throw new DataLinkLayerException(new ContentionErrorException(), "Contention error occured");
                            }
                        }
                        else if (b == (byte) DataLinkControlCodes.NAK)
                        {
                            _errorCounterNAK++;
                            if (_errorCounterNAK >= 6)
                            {
                                _exceptionOccured = new DataLinkLayerException(new BusyException(), "Remote system busy while Establishment Phase");
                                _state = States.TerminationPhase;
                            }
                            else
                            {
                                _state = States.EstablishmentPhaseBegin;
                                _timersManager.StartTimer(WAIT_DELAY_TIMER_NAME, 1000);
                            }
                        }
                        else
                        {
                            _exceptionOccured = new DataLinkLayerException(new DefectiveResponseException(), "Defective response error occured while Establishment Phase");
                            _state = States.TerminationPhase;
                        }
                    }
                    catch (TimeoutException timeoutException)
                    {
                        if (_timersManager.CheckTimerTimeout(WAIT_ANSWER_TIMER_NAME))
                        {
                            _exceptionOccured = new DataLinkLayerException(timeoutException, "Remote device doesn't response while Establishment Phase");
                            _state = States.TerminationPhase;
                        }
                    }

                    break;
                case States.TransferPhaseSendFrame:
                    _errorCounterDefective = 0;
                    _errorCounterNAK = 0;
                    _frameCounter++;
                    if (_frameCounter >= _frames.Length)
                    {
                        _state = States.TerminationPhase;
                        break;
                    }

                    _currentFrame = _frames[_frameCounter];
                    buf = Encoding.ASCII.GetBytes(_currentFrame.ToString());
                    _stream.Write(buf, 0, buf.Length);
                    _log.LogInformation("[send]{0}", ControlCodesUtility.ReplaceControlCodesToLoggingCodes(_currentFrame.ToString()));
                    _timersManager.StartTimer(WAIT_ANSWER_TIMER_NAME, 15000);
                    _state = States.TransferPhaseWaitAnswer;
                    break;
                case States.TransferPhaseResendFrame:
                    buf = Encoding.ASCII.GetBytes(_currentFrame.ToString());
                    _stream.Write(buf, 0, buf.Length);
                    _log.LogInformation("[send]{0}", ControlCodesUtility.ReplaceControlCodesToLoggingCodes(_currentFrame.ToString()));
                    _timersManager.StartTimer(WAIT_ANSWER_TIMER_NAME, 15000);
                    _state = States.TransferPhaseWaitAnswer;
                    break;
                case States.TransferPhaseWaitAnswer:
                    try
                    {
                        _stream.ReadTimeout = 100;
                        var b = (byte) _stream.ReadByte();
                        _timersManager.StopTimer(WAIT_ANSWER_TIMER_NAME);
                        _log.LogInformation("[receive]{0}", ControlCodesUtility.ToControlCode(b));
                        if (b == (byte) DataLinkControlCodes.ACK)
                        {
                            _state = States.TransferPhaseSendFrame;
                        }
                        else if (b == (byte) DataLinkControlCodes.NAK || b == (byte) DataLinkControlCodes.EOT)
                        {
                            _errorCounterNAK++;
                            if (_errorCounterNAK >= 6)
                            {
                                _state = States.TerminationPhase;
                                _exceptionOccured = new DataLinkLayerException(new BusyException(), "Remote system busy while Transfer Phase");
                            }
                            else
                            {
                                _state = States.TransferPhaseResendFrame;
                                _timersManager.StartTimer(WAIT_DELAY_TIMER_NAME, 10000);
                            }
                        }
                        else
                        {
                            _errorCounterDefective++;
                            if (_errorCounterDefective >= 6)
                            {
                                _state = States.TerminationPhase;
                                _exceptionOccured = new DataLinkLayerException(new DefectiveResponseException(), "Remote system response defective while Transfer Phase");
                            }
                            else
                            {
                                _state = States.TransferPhaseResendFrame;
                            }
                        }
                    }
                    catch (TimeoutException timeoutException)
                    {
                        if (_timersManager.CheckTimerTimeout(WAIT_ANSWER_TIMER_NAME))
                        {
                            _state = States.TerminationPhase;
                            _exceptionOccured = new DataLinkLayerException(timeoutException, "Remote device doesn't response while Transfer Phase");
                        }
                    }

                    break;
                case States.TerminationPhase:
                    _stream.WriteByte((byte) DataLinkControlCodes.EOT);
                    _log.LogInformation("[send]{0}", ControlCodesUtility.ToControlCode((byte) DataLinkControlCodes.EOT));
                    if (_exceptionOccured != null)
                    {
                        throw _exceptionOccured;
                    }
                    else
                    {
                        _uploadCompleted = true;
                    }

                    break;
            }
        }
    }
}