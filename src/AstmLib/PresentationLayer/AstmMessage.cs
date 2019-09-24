using System.Collections.Generic;
using System.Text;

namespace AstmLib.PresentationLayer
{
	public class AstmMessage : IEnumerable<AstmRecord>, IEnumerable<string>
	{
        public AstmHeaderRecord HeaderRecord { get; set; }

        public override string ToString()
		{
			var message = new StringBuilder();
			foreach(var record in this)
			{
				message.Append(record + "\r\n");
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
