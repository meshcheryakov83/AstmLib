using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmScientificRecord : AstmRecord
	{
		#region Constructors

		public AstmScientificRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
			Fields = new string[21];
			RecordTypeId = AstmRecordTypeIds.Scientific;
		}

		#endregion

		#region Fields Definition

		public string SequenceNumber
		{
			get { return Fields[1]; }
			set { Fields[1] = value; }
		}

		#endregion
	}
}
