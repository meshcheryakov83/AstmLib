using ApplicationException = AstmLib.DataLinkLayer.Exceptions.ApplicationException;

namespace AstmLib.PresentationLayer.Exceptions
{
    public class AstmMessageBuilderException : ApplicationException
    {
        private AstmMessage[] _successfullyBuildedMessages;

        public AstmMessage[] SuccessfullyBuildedMessages
        {
            get => _successfullyBuildedMessages;
            set => _successfullyBuildedMessages = value;
        }

        public AstmMessageBuilderException(string message, AstmMessage[] successfullyBuildedMessages)
            : base(message)
        {
            _successfullyBuildedMessages = successfullyBuildedMessages;
        }
    }
}