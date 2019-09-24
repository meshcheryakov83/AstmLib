using System;

namespace AstmLib.DataLinkLayer.Exceptions
{
    public class FrameParseException : ApplicationException
    {
        public FrameParseException(Exception innerException, string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        public FrameParseException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }
    }
}