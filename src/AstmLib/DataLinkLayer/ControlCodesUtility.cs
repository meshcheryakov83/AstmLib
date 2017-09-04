using System;
using System.Collections.Generic;
using System.Text;

namespace AstmLib.DataLinkLayer
{
	public static class ControlCodesUtility
	{
		public static string ToControlCode(byte b)
		{
			string ret = string.Format("{0}", b);
			try
			{
				ret = string.Format("<{0}>", ((DataLinkControlCodes)b).ToString());
			}
			catch (Exception)
			{
			}
			return ret;
		}

		public static string ReplaceControlCodesToLoggingCodes(string arg)
		{
			Array controlBytes = Enum.GetValues(typeof(DataLinkControlCodes));

			foreach (byte b in controlBytes)
			{
				if (arg.Contains(((char)b).ToString()))
					arg = arg.Replace(((char)b).ToString(), string.Format("<{0}>", ((DataLinkControlCodes)b).ToString()));
			}

			return arg;
		}
	}
}
