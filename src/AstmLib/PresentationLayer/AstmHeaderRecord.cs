using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmHeaderRecord : AstmRecord
	{
		#region Constructors

		public AstmHeaderRecord(AstmHighLevelSettings highLevelSettings)
            : base(highLevelSettings)
		{
			Fields = new string[14];
			RecordTypeId = AstmRecordTypeIds.Header;
		}

		#endregion

		#region Fields Definition

		public string DelemiterDefenition
		{
			get => Fields[1];
            set => Fields[1] = value;
        }

		public string MessageControlId
		{
			get => Fields[2];
            set => Fields[2] = value;
        }

		public string AccessPassword
		{
			get => Fields[3];
            set => Fields[3] = value;
        }

		public string SenderName
		{
			get => Fields[4];
            set => Fields[4] = value;
        }

		public string SenderStreetAddress
		{
			get => Fields[5];
            set => Fields[5] = value;
        }

		//public string ReservedField
		//{
		//    get { return _fields[6]; }
		//    set { _fields[6] = value; }
		//}

		public string SenderTelephoneNumber
		{
			get => Fields[7];
            set => Fields[7] = value;
        }

		public string ReceiverName
		{
			get => Fields[9];
            set => Fields[9] = value;
        }

		public string Comment
		{
			get => Fields[10];
            set => Fields[10] = value;
        }

		public string ProcessingId
		{
			get => Fields[11];
            set => Fields[11] = value;
        }

		public string VersionNo
		{
			get => Fields[12];
            set => Fields[12] = value;
        }

		public string DateAndTime
		{
			get => Fields[13];
            set => Fields[13] = value;
        }

		#endregion

		public void AddPatient(AstmPatientRecord patient)
		{
			AddChild(patient);
		}

		public void AddQuery(AstmQueryRecord query)
		{
			AddChild(query);
		}

		public void AddComment(AstmCommentRecord comment)
		{
			AddChild(comment);
		}

		public void AddManufacturer(AstmManufaturerRecord manufacturer)
		{
			AddChild(manufacturer);
		}

		public AstmTerminationRecord TerminationRecord
		{
			get => (AstmTerminationRecord)Next;
            set
			{
				_next = value;
				value.SequenceNumber = "1";
			}
		}

		public AstmPatientRecord[] Patients => GetChildrenOfType<AstmPatientRecord>();
    }
}
