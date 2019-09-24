using System;

namespace AstmLib.DataLinkLayer
{
	public class FrameNumberException : Exception
	{
		public FrameNumberException(string format, params object[] args)
			: base(string.Format(format, args))
		{
		}
	}
}
