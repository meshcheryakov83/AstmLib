using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.DataLinkLayer
{
	public static class FrameUtils
	{
		public static string CalcCRC(string text)
		{
			byte crc = 0;
			foreach (char ch in text)
				crc += (byte)ch;

			return crc.ToString("X2");
		}

		public static int IncrementFN(int fn)
		{
			return (fn + 1) % 8;
		}

		public static Frame[] SplitToFrames(string message, AstmLowLevelSettings lowLevelSettings)
		{
			string[] lines = message.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			List<Frame> frames = new List<Frame>();
			int fn = 1;
			foreach (string line in lines)
			{
				string lineCopy = line;
				while (lineCopy.Length > Frame.BODY_MAX_LENGTH)
				{
					string body = lineCopy.Substring(0, Frame.BODY_MAX_LENGTH);
					lineCopy = lineCopy.Substring(Frame.BODY_MAX_LENGTH);
					frames.Add(new Frame(body, fn, FrameTypes.IntermediateFrame, lowLevelSettings));
					fn = IncrementFN(fn);
				}

				if (lineCopy.Length > 0)
					frames.Add(new Frame(lineCopy, fn, FrameTypes.TerminationFrame, lowLevelSettings));
				fn = IncrementFN(fn);
			}

			return frames.ToArray();
		}
	}
}
