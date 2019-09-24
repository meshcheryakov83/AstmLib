using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;
using AstmLib.DataLinkLayer.Exceptions;
using AstmLib.Utilities;
using Microsoft.Extensions.Logging;

namespace AstmLib.DataLinkLayer
{
    public interface IDownloader
    {
        void Reset();
        string Message { get; }
        bool Completed { get; }

        /// <summary>
        /// Читает максимум один фрейм и возвращает управление. При этом сохраняет свое состояние и ждет повторного вызова
        /// чтобы выкачать все сообщение.
        /// </summary>
        /// <exception cref="ASTMIOException">Любая ошибка упаковывается в этот класс как InnerException</exception>
        /// <exception cref="TimeoutException">Тихо в лесу</exception>
        void ExecuteDownloadStep();
    }

    public class Downloader : IDownloader
    {
        private readonly ILogger<Downloader> _log;

		enum States
		{
			WaitEstablishmentPhase,
			ReceiveFrame,
			ProcessReceivedFrame,
			Completed
		}

	    private readonly ITimersManager _timersManager;
		private string _message;
		private bool _completed = false;
		private readonly IAstmChannel _stream = null;
		private States _state;
		private int _previousFN = 0;
		private List<string> _records = new List<string>();
		private string _currentRecord = "";
	    readonly List<byte> _buf = new List<byte>();

		private const string WAIT_FRAME_TIMER_NAME = "timer1";
	    private readonly AstmLowLevelSettings _lowLevelSettings;


        public Downloader(IAstmChannel stream, AstmLowLevelSettings lowLevelSettings, ITimersManager timersManager, ILogger<Downloader> log)
		{
			_stream = stream;
            _lowLevelSettings = lowLevelSettings;
            _timersManager = timersManager;
		    _log = log;
		    _timersManager.CreateTimer(WAIT_FRAME_TIMER_NAME);
		}

		public void Reset()
		{
			_buf.Clear();
			_timersManager.StopAllTimers();
			_message = "";
			_completed = false;
			_state = States.WaitEstablishmentPhase;
			_previousFN = 0;
			_records = new List<string>();
			_currentRecord = "";
		}

		public string Message => _message;

	    public bool Completed => _completed;

	    /// <summary>
		/// Читает максимум один фрейм и возвращает управление. При этом сохраняет свое состояние и ждет повторного вызова
		/// чтобы выкачать все сообщение.
		/// </summary>
		/// <exception cref="ASTMIOException">Любая ошибка упаковывается в этот класс как InnerException</exception>
		/// <exception cref="TimeoutException">Тихо в лесу</exception>
		public void ExecuteDownloadStep()
		{
            if (Completed)
				throw new InvalidOperationException("Download has been completed already");

			switch (_state)
			{
				case States.WaitEstablishmentPhase:
			        _stream.ReadTimeout = 100;
					var b = (byte)_stream.ReadByte();
					if (b == (byte)DataLinkControlCodes.ENQ)
					{
						_log.LogInformation($"[receive]{ControlCodesUtility.ToControlCode(b)}");
						_stream.WriteByte((byte)DataLinkControlCodes.ACK);
						_log.LogInformation($"[send]{ControlCodesUtility.ToControlCode((byte)DataLinkControlCodes.ACK)}");
						_state = States.ReceiveFrame;
					}
					else
					{
						_log.LogDebug($"[receive]{(char)b}");
					}
					break;
				case States.ReceiveFrame:
					if (!_timersManager.IsTimerInStartedState(WAIT_FRAME_TIMER_NAME))
						_timersManager.StartTimer(WAIT_FRAME_TIMER_NAME, _lowLevelSettings.WaitFrameTimeout);
					try
					{
                        _stream.ReadTimeout = 100;
                        b = (byte)_stream.ReadByte();
						if (b == (byte)DataLinkControlCodes.EOT)
						{
							_timersManager.StopTimer(WAIT_FRAME_TIMER_NAME);
							_log.LogInformation("[receive]{0}", ControlCodesUtility.ToControlCode(b));
							_state = States.Completed;
							_completed = true;
							break;
						}
						_buf.Add(b);
						if (b == (byte)DataLinkControlCodes.LF)
						{
							ProcessReceivedFrame();
							_timersManager.StopTimer(WAIT_FRAME_TIMER_NAME);
						}
						_state = States.ReceiveFrame;
					}
					catch (TimeoutException timeoutException)
					{
						if (_timersManager.CheckTimerTimeout(WAIT_FRAME_TIMER_NAME))
							throw new DataLinkLayerException(timeoutException, "Wait frame timeout");
					}
					break;
				case States.Completed:
					throw new InvalidOperationException("Download has been completed already");
            }
        }

		private void ProcessReceivedFrame()
		{
			Frame frame = null;
			Exception frameError = null;
			try
			{
				var decodedString = Encoding.ASCII.GetString(_buf.ToArray(), 0, _buf.Count);
				_log.LogInformation("[receive]{0}", ControlCodesUtility.ReplaceControlCodesToLoggingCodes(decodedString));
				frame = Frame.Parse(decodedString, _lowLevelSettings);
				var expectedFN = (_previousFN + 1) % 8;
				if (frame.FN != expectedFN)
					frameError = new FrameNumberException("Expected frame number {0} but was {1}", expectedFN, frame.FN);
				_previousFN = frame.FN;
			}
			catch (FrameParseException frameParseException)
			{
				_log.LogError(frameParseException.Message);
				frameError = frameParseException;
			}

			_buf.Clear();
			if (frameError == null)
			{
				// Ok
				_currentRecord += frame.Body;
				if (frame.FrameType == FrameTypes.TerminationFrame)
				{
					_message += _currentRecord + "\r\n";
					_currentRecord = "";
				}
				if (!_completed)
				{
					_stream.WriteByte((byte)DataLinkControlCodes.ACK);
					_log.LogInformation("[send]{0}", ControlCodesUtility.ToControlCode((byte)DataLinkControlCodes.ACK));
				}
			}
			else
			{
				if (_completed)
				{
					// Frame error
					_stream.WriteByte((byte)DataLinkControlCodes.NAK);
					_log.LogInformation("[send]{0}", ControlCodesUtility.ToControlCode((byte)DataLinkControlCodes.NAK));
				}
			}
		}
	}
}