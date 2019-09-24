using System;

namespace AstmLib.DataLinkLayer
{
    public interface IAstmIOController
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
        void AddMessageToUploadQueue(string message);
        event EventHandler<IOMessageEventArgs> MessageDownloadCompleted;
        event EventHandler<IOMessageEventArgs> MessageUploadCompleted;
        event EventHandler<IOMessageEventArgs> UnhandledErrorOccured;
    }
}