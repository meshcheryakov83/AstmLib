namespace AstmLib.PresentationLayer.Exceptions
{
	public class InvalidSNAstmMessageBuilderException : AstmMessageBuilderException
	{
		public InvalidSNAstmMessageBuilderException(int lineNumber, string record, AstmMessage[] successfullyBuildedMessages)
			: base($"Invalid SN in line number {lineNumber} record: '{record}'", successfullyBuildedMessages)
		{

		}
	}
}
