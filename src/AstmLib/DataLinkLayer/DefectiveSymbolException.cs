using System;
using AstmLib.DataLinkLayer.Exceptions;
using ApplicationException = AstmLib.DataLinkLayer.Exceptions.ApplicationException;

namespace AstmLib.DataLinkLayer
{
	public class DefectiveSymbolException : ApplicationException
	{
		public DefectiveSymbolException(string format, params object[] args)
			: base(string.Format(format, args))
		{
		}
	}
}