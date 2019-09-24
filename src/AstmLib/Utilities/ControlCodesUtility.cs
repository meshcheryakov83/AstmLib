using System;
using AstmLib.DataLinkLayer;

namespace AstmLib.Utilities
{
    public static class ControlCodesUtility
    {
        public static string ToControlCode(byte b)
        {
            var ret = $"{b}";
            try
            {
                ret = $"<{((DataLinkControlCodes) b).ToString()}>";
            }
            catch (Exception)
            {
            }

            return ret;
        }

        public static string ReplaceControlCodesToLoggingCodes(string arg)
        {
            var controlBytes = Enum.GetValues(typeof(DataLinkControlCodes));

            foreach (byte b in controlBytes)
            {
                if (arg.Contains(((char) b).ToString()))
                {
                    arg = arg.Replace(((char) b).ToString(), $"<{((DataLinkControlCodes) b).ToString()}>");
                }
            }

            return arg;
        }
    }
}