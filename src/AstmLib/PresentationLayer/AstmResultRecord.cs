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

        public AstmOrderRecord Order => (AstmOrderRecord) Parent;

        #region Fields Definition

        public string SequenceNumber
        {
            get => Fields[1];
            set => Fields[1] = value;
        }

        public string UniversalTestId
        {
            get => Fields[2];
            set => Fields[2] = value;
        }

        public string MeasurementValue
        {
            get => Fields[3];
            set => Fields[3] = value;
        }

        public string Units
        {
            get => Fields[4];
            set => Fields[4] = value;
        }

        public string ResultAbnormalFlags
        {
            get => Fields[6];
            set => Fields[6] = value;
        }

        public string ResultStatus
        {
            get => Fields[8];
            set => Fields[8] = value;
        }

        public string DateTime
        {
            get => Fields[12];
            set => Fields[12] = value;
        }

        #endregion
    }
}