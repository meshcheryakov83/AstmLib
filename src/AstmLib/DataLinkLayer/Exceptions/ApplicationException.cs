using System;

namespace AstmLib.DataLinkLayer.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message = null, Exception innerEx = null)
            : base(message, innerEx)
        {

        }
    }
}