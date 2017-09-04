using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstmLib.DataLinkLayer;
using AstmLib.Configuration;
using AstmLib.DataLinkLayer.Exceptions;
using ApplicationException = System.ApplicationException;

namespace AstmLib.PresentationLayer
{
	public abstract class AstmRecord
	{
		#region Fields

		protected string[] _fields;
		protected AstmRecord _firstChild;
		protected AstmRecord _next;
		protected AstmRecord _parent;
	    private AstmHighLevelSettings _highLevelSettings;

	    protected AstmRecord(AstmHighLevelSettings highLevelSettings)
	    {
	        _highLevelSettings = highLevelSettings;
	    }

	    #endregion

        #region Public methods and properties

        #region Record's content 

        public string[] Fields
		{
			get { return _fields; }
			set { _fields = value; }
		}

		public string RecordTypeId
		{
			get { return _fields[0]; }
			set { _fields[0] = value; }
		}

		public string GetPart(int fieldNumber, int partNumber)
		{
			if (Fields.Length <= fieldNumber)
				throw new IndexOutOfRangeException(string.Format("Field number {0} doesn't exist", fieldNumber));
			if (Fields[fieldNumber] == null)
				return null;
			var snParts = Fields[fieldNumber].Split(new[] { '^' }, StringSplitOptions.None);
			if (snParts.Length <= partNumber)
			{
				return null;
				//throw new IndexOutOfRangeException(string.Format("Part {0} doesn't exist in field {1}", partNumber, fieldNumber));
			}
				
			return snParts[partNumber].Trim();
		}

		public string GetField(int fieldNumber)
		{
			if (Fields.Length <= fieldNumber)
				throw new IndexOutOfRangeException(string.Format("Field number {0} doesn't exist", fieldNumber));
			return Fields[fieldNumber].Trim();
		}

		#endregion

		#region Navigation

		public AstmRecord Next
		{
			get { return _next; }
		}

		public AstmRecord FirstChild
		{
			get { return _firstChild; }
		}

		public AstmRecord[] Children
		{
			get
			{
				List<AstmRecord> children = new List<AstmRecord>();
				AstmRecord child = _firstChild;
				while(child!=null)
				{
					children.Add(child);
					child = child.Next;
				}
				return children.ToArray();
			}
		}

		public bool HasChildren
		{
			get
			{
				return _firstChild != null;
			}
		}

		protected T[] GetChildrenOfType<T>()
			where T : AstmRecord
		{
			List<T> children = new List<T>();
			foreach (AstmRecord child in Children)
			{
				if (child is T)
					children.Add((T)child);
			}

			return children.ToArray();
		}

		public AstmRecord Parent
		{
			get { return _parent; }
		}

		#endregion

		#region Manipulation
		
		public void AddChild(AstmRecord record)
		{
		    int seqNum = 0;
		    if (_firstChild == null)
		    {
		        _firstChild = record;
		    }
		    else
		    {
		        AstmRecord current = _firstChild;
				
		        if (current.RecordTypeId == record.RecordTypeId)
		            seqNum++;
		        while (current._next != null)
		        {
		            current = current._next;

		            if (current.RecordTypeId == record.RecordTypeId)
		                seqNum++;
		        }

		        current._next = record;
		    }
		    record._parent = this;
		    record._fields[1] = (seqNum+1).ToString();
		}
		
		public void RemoveChild(AstmRecord record)
		{
			throw new InvalidOperationException("This operation doesn't implementd yet");
		}

		#endregion

		public override string ToString()
		{
			string line = "";
			foreach (string field in _fields)
			{
				line += field + "|";
			}

			if (_highLevelSettings.TrimRecords)
				line = line.TrimEnd(new char[] { '|' });

			// Each record ends with <CR>
			return line + ((char)DataLinkControlCodes.CR).ToString();
		}

		#endregion

		#region Static methods

		public static AstmRecord Parse(string line, AstmHighLevelSettings highLevelSettings)
		{
			AstmRecord record = null;
			switch (line[0])
			{
				case 'H':
					record = new AstmHeaderRecord(highLevelSettings);
					break;
				case 'O':
					record = new AstmOrderRecord(highLevelSettings);
					break;
				case 'P':
					record = new AstmPatientRecord(highLevelSettings);
					break;
				case 'R':
					record = new AstmResultRecord(highLevelSettings);
					break;
				case 'C':
					record = new AstmCommentRecord(highLevelSettings);
					break;
				case 'M':
					record = new AstmManufaturerRecord(highLevelSettings);
					break;
				case 'L':
					record = new AstmTerminationRecord(highLevelSettings);
					break;
				case 'Q':
					record = new AstmQueryRecord(highLevelSettings);
					break;
				default:
					throw new ApplicationException(string.Format("Unknown astm record type '{0}'", line[0]));
			}

			//if (record.RecordTypeId != Activator.CreateInstance<T>().RecordTypeId)
			//    throw new BuildAstmRecordException("Record ID is invalid");

			return fillFields(line, record);
		}

		public static T Parse<T>(string line, AstmHighLevelSettings highLevelSettings)
			where T : AstmRecord
		{
			AstmRecord record = Parse(line, highLevelSettings);
			if (!(record is T))
				throw new ApplicationException(string.Format("Type of record is not {0}", typeof(T).Name));

			return (T)record;
		}

		private static AstmRecord fillFields(string arg, AstmRecord record)
		{
			arg = arg.Trim(new char[] { '\r', '\n' });
			string[] fields = arg.Split(new char[] { '|' });

			if (fields.Length > record._fields.Length)
			{
				if (fields.Length == record._fields.Length + 1 || fields.Last() == "")
				{
					// It's ok if last field was "" after |
					// for example:
					//H|\^&|7923||cobas 8000^1.03|||||host|RSUPL|P|1|20140801182713|
				}
				else
				{
					throw new Exception("The number of fields is exceeded the maximum nubber allowed for this type of record");	
				}
			}
				

			for (int i = 0; i < fields.Length && i < record.Fields.Length; i++)
				record._fields[i] = fields[i];
			return record;
		}

		#endregion
	}
}
