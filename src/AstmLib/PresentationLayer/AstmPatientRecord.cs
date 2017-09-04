using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmPatientRecord : AstmRecord
	{
		#region Constructors

		public AstmPatientRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
			Fields = new string[35];
			RecordTypeId = AstmRecordTypeIds.Patient;
		}

		#endregion

		#region Fields Definition

		public string SequenceNumber
		{
			get { return Fields[1]; }
			set { Fields[1] = value; }
		}

		public string PracticeAssignedPatientId
		{
			get { return Fields[2]; }
			set { Fields[2] = value; }
		}

		public string LaboratoryAssignedPatientId
		{
			get { return Fields[3]; }
			set { Fields[3] = value; }
		}

		public string PatientId
		{
			get { return Fields[4]; }
			set { Fields[4] = value; }
		}

		#endregion

		//public void AddOrder(AstmOrderRecord order)
		//{
		//    this.AddChild(order);
		//}

		//public void AddComment(AstmCommentRecord comment)
		//{
		//    this.AddChild(comment);
		//}

		public AstmOrderRecord[] Orders
		{
			get
			{
				return GetChildrenOfType<AstmOrderRecord>();
			}
		}

		public AstmCommentRecord[] Comments
		{
			get
			{
				return GetChildrenOfType<AstmCommentRecord>();
			}
		}
	}
}
