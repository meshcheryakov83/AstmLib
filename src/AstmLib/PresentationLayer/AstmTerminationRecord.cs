using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
    public class AstmTerminationRecord : AstmRecord
    {
        #region Constructors

        public AstmTerminationRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
            Fields = new string[3];
            RecordTypeId = AstmRecordTypeIds.Termination;
        }

        #endregion

        #region Fields Definition

        public string SequenceNumber
        {
            get => Fields[1];
            set => Fields[1] = value;
        }

        public string TerminationCode
        {
            get => Fields[2];
            set => Fields[2] = value;
        }

        #endregion
    }
}