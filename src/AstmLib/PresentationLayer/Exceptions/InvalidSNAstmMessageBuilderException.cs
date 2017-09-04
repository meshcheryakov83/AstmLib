using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.PresentationLayer;

namespace AstmLib.PresentationLayer.Exceptions
{
	public class InvalidSNAstmMessageBuilderException : AstmMessageBuilderException
	{
		public InvalidSNAstmMessageBuilderException(int lineNumber, string record, AstmMessage[] successfullyBuildedMessages)
			: base(string.Format("Invalid SN in line number {0} record: '{1}'", lineNumber, record), successfullyBuildedMessages)
		{

		}
	}
}
