using System;
using System.Collections.Generic;
using System.Text;
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
			this.RecordTypeId = AstmRecordTypeIds.Header;
		}

		#endregion

		#region Fields Definition

		public string DelemiterDefenition
		{
			get { return Fields[1]; }
			set { Fields[1] = value; }
		}

		public string MessageControlId
		{
			get { return Fields[2]; }
			set { Fields[2] = value; }
		}

		public string AccessPassword
		{
			get { return Fields[3]; }
			set { Fields[3] = value; }
		}

		public string SenderName
		{
			get { return Fields[4]; }
			set { Fields[4] = value; }
		}

		public string SenderStreetAddress
		{
			get { return Fields[5]; }
			set { Fields[5] = value; }
		}

		//public string ReservedField
		//{
		//    get { return _fields[6]; }
		//    set { _fields[6] = value; }
		//}

		public string SenderTelephoneNumber
		{
			get { return Fields[7]; }
			set { Fields[7] = value; }
		}

		public string ReceiverName
		{
			get { return Fields[9]; }
			set { Fields[9] = value; }
		}

		public string Comment
		{
			get { return Fields[10]; }
			set { Fields[10] = value; }
		}

		public string ProcessingId
		{
			get { return Fields[11]; }
			set { Fields[11] = value; }
		}

		public string VersionNo
		{
			get { return Fields[12]; }
			set { Fields[12] = value; }
		}

		public string DateAndTime
		{
			get { return Fields[13]; }
			set { Fields[13] = value; }
		}

		#endregion

		public void AddPatient(AstmPatientRecord patient)
		{
			this.AddChild(patient);
		}

		public void AddQuery(AstmQueryRecord query)
		{
			this.AddChild(query);
		}

		public void AddComment(AstmCommentRecord comment)
		{
			this.AddChild(comment);
		}

		public void AddManufacturer(AstmManufaturerRecord manufacturer)
		{
			this.AddChild(manufacturer);
		}

		public AstmTerminationRecord TerminationRecord
		{
			get { return (AstmTerminationRecord)Next; }
			set
			{
				_next = value;
				value.SequenceNumber = "1";
			}
		}

		public AstmPatientRecord[] Patients
		{
			get
			{
				return GetChildrenOfType<AstmPatientRecord>();
			}
		}
	}
}
