using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.PresentationLayer;

namespace AstmLib.PresentationLayer
{
	public class AstmRecordEnumerator : IEnumerator<AstmRecord>, IEnumerator<string>
	{
		private AstmMessage _message;
		private AstmRecord _current;

		#region IEnumerator<AstmRecord> Members

		public AstmRecord Current
		{
			get { return _current; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_message = null;
			_current = null;
		}

		#endregion

		#region IEnumerator Members

		object System.Collections.IEnumerator.Current
		{
			get { return _current; }
		}

		public bool MoveNext()
		{
			if (_current == null)
				_current = _message.HeaderRecord;
			else if (_current.FirstChild != null)
				_current = _current.FirstChild;
			else if (_current.Next != null)
				_current = _current.Next;
			else
			{
				while (_current != null && _current.Next == null)
					_current = _current.Parent;

				if (_current == null)
					return false;
				else
					_current = _current.Next;
			}

			return true;
		}

		public void Reset()
		{
			_current = null;
		}

		public AstmRecordEnumerator(AstmMessage message)
		{
			_message = message;
			Reset();
		}

		#endregion

		#region IEnumerator<string> Members

		string IEnumerator<string>.Current
		{
			get { return _current.ToString(); }
		}

		#endregion
	}
}
