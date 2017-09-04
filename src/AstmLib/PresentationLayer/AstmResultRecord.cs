using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmResultRecord : AstmRecord
	{
		#region Constructors

		public AstmResultRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
		{
			Fields = new string[16];
			RecordTypeId = AstmRecordTypeIds.Result;
		}

		#endregion

		public AstmOrderRecord Order
		{
			get { return (AstmOrderRecord)Parent; }
		}

		#region Fields Definition

		public string SequenceNumber
		{
			get { return Fields[1]; }
			set { Fields[1] = value; }
		}

		public string UniversalTestId
		{
			get { return Fields[2]; }
			set { Fields[2] = value; }
		}

		public string MeasurementValue
		{
			get { return Fields[3]; }
			set { Fields[3] = value; }
		}

		public string Units
		{
			get { return Fields[4]; }
			set { Fields[4] = value; }
		}

		public string ResultAbnormalFlags
		{
			get { return Fields[6]; }
			set { Fields[6] = value; }
		}

		public string ResultStatus
		{
			get { return Fields[8]; }
			set { Fields[8] = value; }
		}

		public string DateTime
		{
			get { return Fields[12]; }
			set { Fields[12] = value; }
		}

		#endregion
	}
}
