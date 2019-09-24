using System;
using System.Collections.Generic;
using System.Linq;
using AstmLib.DataLinkLayer;
using AstmLib.Configuration;
using ApplicationException = System.ApplicationException;

namespace AstmLib.PresentationLayer
{
	public abstract class AstmRecord
	{
		#region Fields

        protected AstmRecord _firstChild;
		protected AstmRecord _next;
		protected AstmRecord _parent;
	    private readonly AstmHighLevelSettings _highLevelSettings;

	    protected AstmRecord(AstmHighLevelSettings highLevelSettings)
	    {
	        _highLevelSettings = highLevelSettings;
	    }

	    #endregion

        #region Public methods and properties

        #region Record's content 

        public string[] Fields { get; protected set; }

        public string RecordTypeId
		{
			get => Fields[0];
            set => Fields[0] = value;
        }

		public string GetPart(int fieldNumber, int partNumber)
		{
			if (Fields.Length <= fieldNumber)
				throw new IndexOutOfRangeException($"Field number {fieldNumber} doesn't exist");
			if (Fields[fieldNumber] == null)
				return null;
			var snParts = Fields[fieldNumber].Split(new[] { '^' }, StringSplitOptions.None);
			if (snParts.Length <= partNumber)
			{
				throw new IndexOutOfRangeException($"Part {partNumber} doesn't exist in field {fieldNumber}");
			}
				
			return snParts[partNumber].Trim();
		}

		public string GetField(int fieldNumber)
		{
			if (Fields.Length <= fieldNumber)
				throw new IndexOutOfRangeException($"Field number {fieldNumber} doesn't exist");
			return Fields[fieldNumber].Trim();
		}

		#endregion

		#region Navigation

		public AstmRecord Next => _next;

        public AstmRecord FirstChild => _firstChild;

        public AstmRecord[] Children
		{
			get
			{
				var children = new List<AstmRecord>();
				var child = _firstChild;
				while(child!=null)
				{
					children.Add(child);
					child = child.Next;
				}
				return children.ToArray();
			}
		}

		public bool HasChildren => _firstChild != null;

        protected T[] GetChildrenOfType<T>()
			where T : AstmRecord
		{
			var children = new List<T>();
			foreach (var child in Children)
			{
				if (child is T)
					children.Add((T)child);
			}

			return children.ToArray();
		}

		public AstmRecord Parent => _parent;

        #endregion

		#region Manipulation
		
		public void AddChild(AstmRecord record)
		{
		    var seqNum = 0;
		    if (_firstChild == null)
		    {
		        _firstChild = record;
		    }
		    else
		    {
		        var current = _firstChild;
				
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
		    record.Fields[1] = (seqNum+1).ToString();
		}

        #endregion

		public override string ToString()
		{
			var line = "";
			foreach (var field in Fields)
			{
				line += field + "|";
			}

			if (_highLevelSettings.TrimRecords)
				line = line.TrimEnd(new char[] { '|' });

			// Each record ends with <CR>
			return line + ((char)DataLinkControlCodes.CR);
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
					throw new ApplicationException($"Unknown astm record type '{line[0]}'");
			}

			return fillFields(line, record);
		}

		public static T Parse<T>(string line, AstmHighLevelSettings highLevelSettings)
			where T : AstmRecord
		{
			var record = Parse(line, highLevelSettings);
			if (!(record is T))
				throw new ApplicationException($"Type of record is not {typeof(T).Name}");

			return (T)record;
		}

		private static AstmRecord fillFields(string arg, AstmRecord record)
		{
			arg = arg.Trim(new char[] { '\r', '\n' });
			var fields = arg.Split(new char[] { '|' });

			if (fields.Length > record.Fields.Length)
			{
				if (fields.Length == record.Fields.Length + 1 || fields.Last() == "")
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
				

			for (var i = 0; i < fields.Length && i < record.Fields.Length; i++)
				record.Fields[i] = fields[i];
			return record;
		}

		#endregion
	}
}
