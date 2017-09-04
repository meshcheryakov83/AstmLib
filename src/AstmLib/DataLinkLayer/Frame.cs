using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;
using AstmLib.DataLinkLayer.Exceptions;

namespace AstmLib.DataLinkLayer
{
	public enum FrameTypes { IntermediateFrame, TerminationFrame }

	public class Frame
	{
		public const int BODY_MAX_LENGTH = 240;

		private string _body;
		private int _fn;
		private FrameTypes _frameType;

		public string Body
		{
			get { return this._body; }
		}

		public int FN
		{
			get { return this._fn; }
		}

		public FrameTypes FrameType
		{
			get { return this._frameType; }
		}

	    private readonly AstmLowLevelSettings _lowLevelSettings;

        public Frame(string body, int fn, FrameTypes frameType, AstmLowLevelSettings lowLevelSettings)
        {
            _lowLevelSettings = lowLevelSettings;

			if (string.IsNullOrEmpty(body))
				throw new FrameConstructionException("Can not construct frame", new ArgumentNullException("Body is null or empty"));
			if (body.Length > _lowLevelSettings.MaxFrameLength)
				throw new FrameConstructionException("Can not construct frame", new ArgumentException("Body length is exceeded 240"));
			if (fn < 0 || fn > 7)
				throw new FrameConstructionException("Can not construct frame", new ArgumentOutOfRangeException("FN must be between 0..7"));

			this._body = body;
			this._fn = fn;
			this._frameType = frameType;
            _lowLevelSettings = lowLevelSettings;
		}

		/// <summary>
		/// Parse string and retrive frame. Note that parsing include checking CRC, and in case of CRC is not equal to calculated
		/// than method throw exception
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		/// <exception cref="FrameParseException"></exception>
		public static Frame Parse(string line, AstmLowLevelSettings lowLevelSettings)
		{
			// Trim STX
			line = line.TrimStart(new char[] { ((char)0x02) });

			//Trim CR LF
			line = line.TrimEnd(new char[] { (char)0x0D, (char)0xA });

			// Extract FN
			int fn = -1;
			try
			{
				fn = int.Parse(line[0].ToString());
			}
			catch (FormatException)
			{
				throw new FrameParseException("Can not parse FN in frame '{0}'", line);
			}

			// Extract CRC
			string crc = line.Substring(line.Length - 2);

			// Extract terminator
			char terminator = line.Substring(line.Length - 3, 1)[0];
			FrameTypes frameType = FrameTypes.TerminationFrame;
			if (terminator == (char)0x17)
				frameType = FrameTypes.IntermediateFrame;
			else if (terminator == (char)0x03)
				frameType = FrameTypes.TerminationFrame;
			else
				throw new FrameParseException("Can not parse terminator in frame '{0}'", line);

			// Extract body
			string body = line.Substring(1, line.Length - 4);

			Frame frame = null;
			try
			{
				frame = new Frame(body, fn, frameType, lowLevelSettings);
			}
			catch(FrameConstructionException frameConstructionException)
			{
				throw new FrameParseException(frameConstructionException, "Error while frame constructioning occured");
			}
			return frame;
		}
		
		public override string ToString()
		{
			string text = _fn.ToString() +
				_body + (_frameType == FrameTypes.IntermediateFrame ? (char)DataLinkControlCodes.ETB : (char)DataLinkControlCodes.ETX);

			string crc = FrameUtils.CalcCRC(text);
			return (char)DataLinkControlCodes.STX + text + crc + 
				(char)DataLinkControlCodes.CR + (char)DataLinkControlCodes.LF;
		}
	}
}
