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
			get => Fields[1];
            set => Fields[1] = value;
        }

		public string PracticeAssignedPatientId
		{
			get => Fields[2];
            set => Fields[2] = value;
        }

		public string LaboratoryAssignedPatientId
		{
			get => Fields[3];
            set => Fields[3] = value;
        }

		public string PatientId
		{
			get => Fields[4];
            set => Fields[4] = value;
        }

		#endregion

		public AstmOrderRecord[] Orders => GetChildrenOfType<AstmOrderRecord>();

        public AstmCommentRecord[] Comments => GetChildrenOfType<AstmCommentRecord>();
    }
}
