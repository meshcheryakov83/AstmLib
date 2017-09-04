using System;
using System.Collections.Generic;
using System.Text;

namespace AstmLib.DataLinkLayer.Exceptions
{
	public class DataLinkLayerException : ApplicationException
	{
		public DataLinkLayerException(Exception innerException, string message, params object[] args)
			: base(string.Format(message, args), innerException)
		{
		}

		public DataLinkLayerException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
	}
}
