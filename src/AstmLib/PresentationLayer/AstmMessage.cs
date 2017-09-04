using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.PresentationLayer;

namespace AstmLib.PresentationLayer
{
	public class AstmMessage : IEnumerable<AstmRecord>, IEnumerable<string>
	{
		AstmHeaderRecord _headerRecord;
		public AstmHeaderRecord HeaderRecord
		{
			get { return _headerRecord; }
			set { _headerRecord = value; }
		}
		
		public override string ToString()
		{
			StringBuilder message = new StringBuilder();
			foreach(AstmRecord record in this)
			{
				message.Append(record.ToString() + "\r\n");
			}
			return message.ToString();
		}

		#region IEnumerable<AstmRecord> Members

		public IEnumerator<AstmRecord> GetEnumerator()
		{
			return new AstmRecordEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new AstmRecordEnumerator(this);
		}

		#endregion

		#region IEnumerable<string> Members

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return new AstmRecordEnumerator(this);
		}

		#endregion
	}
}
