using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.DataLinkLayer.Exceptions;
using AstmLib.PresentationLayer;
using ApplicationException = AstmLib.DataLinkLayer.Exceptions.ApplicationException;

namespace AstmLib.PresentationLayer.Exceptions
{
	public class AstmMessageBuilderException : ApplicationException
	{
		private AstmMessage[] _successfullyBuildedMessages;
		public AstmMessage[] SuccessfullyBuildedMessages
		{
			get { return _successfullyBuildedMessages; }
			set { _successfullyBuildedMessages = value; }
		}

		public AstmMessageBuilderException(string message, AstmMessage[] successfullyBuildedMessages)
			: base(message)
		{
			_successfullyBuildedMessages = successfullyBuildedMessages;
		}
	}
}
