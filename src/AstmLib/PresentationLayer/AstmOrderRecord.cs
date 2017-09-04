using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmOrderRecord : AstmRecord
	{
		#region Constructors

		public AstmOrderRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
			Fields = new string[31];
			RecordTypeId = AstmRecordTypeIds.Order;
		}

		#endregion

		#region Fields Definition

		public string SequenceNumber
		{
			get { return Fields[1]; }
			set { Fields[1] = value; }
		}

		public string SpecimenId
		{
			get { return Fields[2]; }
			set { Fields[2] = value; }
		}

		public string InstrumentSpecimenId
		{
			get { return Fields[3]; }
			set { Fields[3] = value; }
		}

		public string UniversalTestId
		{
			get { return Fields[4]; }
			set { Fields[4] = value; }
		}

		public string Priority
		{
			get { return Fields[5]; }
			set { Fields[5] = value; }
		}

		public string ActionCode
		{
			get { return Fields[11]; }
			set { Fields[11] = value; }
		}

		public string SpecimenDescriptor
		{
			get { return Fields[15]; }
			set { Fields[15] = value; }
		}

		public string ReportTypes
		{
			get { return Fields[25]; }
			set { Fields[25] = value; }
		}

		#endregion

		public AstmResultRecord[] Results
		{
			get
			{
				return GetChildrenOfType<AstmResultRecord>();
			}
		}

		//public void AddComment(AstmCommentRecord comment)
		//{
		//    this.AddChild(comment);
		//}

		//public void AddResult(AstmResultRecord result)
		//{
		//    this.AddChild(result);
		//}
	}
}
