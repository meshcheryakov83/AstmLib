using System;
using System.Collections.Generic;
using System.Text;
using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
	public class AstmCommentRecord : AstmRecord
	{
		#region Constructors

		public AstmCommentRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
			Fields = new string[5];
			RecordTypeId = AstmRecordTypeIds.Comment;
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
