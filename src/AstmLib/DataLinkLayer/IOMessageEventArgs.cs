using System;

namespace AstmLib.DataLinkLayer
{
    public class IOMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception OccuredException { get; set; } = null;

        public IOMessageEventArgs(string message, Exception occuredException)
        {
            Message = message;
            OccuredException = occuredException;
        }

        public IOMessageEventArgs(string message)
            : this(message, null)
        {
        }
    }
}