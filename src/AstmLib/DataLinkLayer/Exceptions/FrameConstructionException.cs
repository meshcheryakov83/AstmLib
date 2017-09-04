using System;
using System.Collections.Generic;
using System.Text;

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
