using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AstmLib.Configuration;
using AstmLib.DataLinkLayer.Exceptions;
using Microsoft.Extensions.Logging;

namespace AstmLib.DataLinkLayer
{
    public class AstmIOController
	{
        enum DataLinkStates { Upload, Download, Neutral }
        public const string DISABLE_UPLOAD_TIMER_NAME = "DisableUpload";

        #region Fields

	    private readonly ILogger<AstmIOController> _log;
		private readonly Queue<string> _uploadQueue = new Queue<string>();
        private readonly IUploader _uploader;
        private readonly IDownloader _downloader;
		private readonly IAstmChannel _stream = null;
	    private readonly AstmLowLevelSettings _lowLevelSettings;
	    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ITimersManager _timersManager;

        private DataLinkStates _state;
        private bool _uploadEnabled = true;

        #endregion

        public bool IsRunning { get; private set; }
        public event EventHandler<IOMessageEventArgs> MessageDownloadCompleted;
        public event EventHandler<IOMessageEventArgs> MessageUploadCompleted;
        public event EventHandler<IOMessageEventArgs> UnhandledErrorOccured;

        #region Constructors & Destructors
        
	    public AstmIOController(
	        IAstmChannel stream,
	        AstmLowLevelSettings lowLevelSettings,
	        ILoggerFactory factory) : this(
                stream,
                lowLevelSettings,
                new Uploader(stream, lowLevelSettings, new TimersManager(), factory.CreateLogger<Uploader>()),
                new Downloader(stream, lowLevelSettings, new TimersManager(), factory.CreateLogger<Downloader>()),
                new TimersManager(),
                factory.CreateLogger<AstmIOController>())
	    {
	    }

	    protected AstmIOController(
            IAstmChannel stream,
            AstmLowLevelSettings lowLevelSettings,
            IUploader uploader,
            IDownloader downloader,
            ITimersManager timersManager,
            ILogger<AstmIOController> log)
	    {
	        _stream = stream;
		    _uploader = uploader;
		    _downloader = downloader;
		    _lowLevelSettings = lowLevelSettings;
            _timersManager = timersManager;
		    _log = log;
		    _timersManager.CreateTimer(DISABLE_UPLOAD_TIMER_NAME);
        }

		#endregion

		public void Start()
		{
            IsRunning = true;
		    Task.Run(
                () => run(_cancellationTokenSource.Token),
                _cancellationTokenSource.Token);
		}

		public void Stop()
		{
            _cancellationTokenSource.Cancel();
            _log.LogInformation("Stopping controller...");
		}

		public void AddMessageToUploadQueue(string message)
		{
		    lock (_uploadQueue)
		    {
		        _uploadQueue.Enqueue(message);
		    }
		}

        private void disableUpload(int millisecond)
		{
			_uploadEnabled = false;
            _timersManager.StartTimer(DISABLE_UPLOAD_TIMER_NAME, millisecond);
		}

	    public void ProcessCurrentState(IDownloader downloader, IUploader uploader)
	    {
	        if (!_timersManager.CheckTimerTimeout(DISABLE_UPLOAD_TIMER_NAME))
	        {
	            _uploadEnabled = true;
	        }

	        if (_stream.IsInFaultState)
	        {
	            if (!_stream.Reopen())
	            {
	                return;
	            }
	        }

            switch (_state)
            {
                case DataLinkStates.Upload:
                    try
                    {
                        _uploader.ExecuteUploadStep();
                        if (_uploader.Completed)
                        {
                            disableUpload(_lowLevelSettings.DelayUploadAfterUpload);
                            _state = DataLinkStates.Neutral;
                            raiseMessageUploadCompleted(new IOMessageEventArgs(_uploader.Message));
                        }
                    }
                    catch (DataLinkLayerException ex)
                    {
                        _log.LogError(ex.Message);
                        if (ex.InnerException is ContentionErrorException)
                        {
                            _downloader.Reset();
                            _state = DataLinkStates.Download;
                        }
                        else
                        {
                            disableUpload(_lowLevelSettings.DelayUploadAfterUploadError);
                            _state = DataLinkStates.Neutral;
                            raiseMessageUploadCompleted(new IOMessageEventArgs(_uploader.Message, ex));
                        }
                    }
                    break;
                case DataLinkStates.Download:
                    try
                    {
                        _downloader.ExecuteDownloadStep();
                        if (_downloader.Completed)
                        {
                            disableUpload(_lowLevelSettings.DelayUploadAfterDownload);
                            _state = DataLinkStates.Neutral;
                            raiseMessageDownloadCompleted(new IOMessageEventArgs(_downloader.Message));
                        }
                    }
                    catch (TimeoutException)
                    {
                        // The Data Link in the neutral state
                        _state = DataLinkStates.Neutral;
                    }
                    catch (DataLinkLayerException ex)
                    {
                        _log.LogError(ex.Message);
                        _state = DataLinkStates.Neutral;
                        raiseMessageDownloadCompleted(new IOMessageEventArgs(_downloader.Message, ex));
                    }
                    break;
                case DataLinkStates.Neutral:
                    if (_uploadQueue.Count > 0 && _uploadEnabled)
                    {
                        string message = "";
                        lock (_uploadQueue)
                        {
                            message = _uploadQueue.Dequeue();
                        }
                        _uploader.ResetAndUpload(message);
                        _state = DataLinkStates.Upload;
                    }
                    else
                    {
                        _downloader.Reset();
                        _state = DataLinkStates.Download;
                    }
                    break;
            }
        }

		private void run(CancellationToken ct)
		{
            _state = DataLinkStates.Neutral;
		    try
		    {
		        while (!ct.IsCancellationRequested)
		        {
                    ProcessCurrentState(_downloader, _uploader);
                }
            }
		    catch (Exception ex)
		    {
		        _log.LogError($"Error occurred and AstmIOController will be stopped: {ex}");
                raiseUnhandledErrorOccured(new IOMessageEventArgs(_downloader.Message, ex));
		    }
		    finally
		    {
                _log.LogInformation("Controller stopped");
                IsRunning = false;
		    }
		}
		
		private void raiseMessageUploadCompleted(IOMessageEventArgs args)
		{
            Task.Run(() =>
            {
                try
                {
                    MessageUploadCompleted?.Invoke(this, args);
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error occured while processing uploaded message: {ex}");
                }
            });
        }

        private void raiseMessageDownloadCompleted(IOMessageEventArgs args)
		{
            Task.Run(() =>
            {
                try
                {
                    MessageDownloadCompleted?.Invoke(this, args);
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error occured while processing downloaded message: {ex}");
                }
            });
		}

        private void raiseUnhandledErrorOccured(IOMessageEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    UnhandledErrorOccured?.Invoke(this, args);
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error occured while processing unhandled error: {ex}");
                }
            });
        }
    }
}
