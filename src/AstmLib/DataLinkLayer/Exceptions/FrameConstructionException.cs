using System;

namespace AstmLib.DataLinkLayer.Exceptions
{
    public class FrameConstructionException : Exception
    {
        public FrameConstructionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}