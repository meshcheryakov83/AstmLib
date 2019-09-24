using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
    public class AstmOrderRecord : AstmRecord
    {
        #region Constructors

        public AstmOrderRecord(AstmHighLevelSettings highLevelSettings)
            : base(highLevelSettings)
        {
            Fields = new string[31];
            RecordTypeId = AstmRecordTypeIds.Order;
        }

        #endregion

        #region Fields Definition

        public string SequenceNumber
        {
            get => Fields[1];
            set => Fields[1] = value;
        }

        public string SpecimenId
        {
            get => Fields[2];
            set => Fields[2] = value;
        }

        public string InstrumentSpecimenId
        {
            get => Fields[3];
            set => Fields[3] = value;
        }

        public string UniversalTestId
        {
            get => Fields[4];
            set => Fields[4] = value;
        }

        public string Priority
        {
            get => Fields[5];
            set => Fields[5] = value;
        }

        public string ActionCode
        {
            get => Fields[11];
            set => Fields[11] = value;
        }

        public string SpecimenDescriptor
        {
            get => Fields[15];
            set => Fields[15] = value;
        }

        public string ReportTypes
        {
            get => Fields[25];
            set => Fields[25] = value;
        }

        #endregion

        public AstmResultRecord[] Results => GetChildrenOfType<AstmResultRecord>();
    }
}