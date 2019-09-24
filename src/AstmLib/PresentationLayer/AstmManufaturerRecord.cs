using AstmLib.Configuration;

namespace AstmLib.PresentationLayer
{
    public class AstmManufaturerRecord : AstmRecord
    {
        #region Constructors

        public AstmManufaturerRecord(AstmHighLevelSettings highLevelSettings) : base(highLevelSettings)
        {
            Fields = new string[200];
            RecordTypeId = AstmRecordTypeIds.Manufacturer;
        }

        #endregion

        #region Fields Definition

        public string SequenceNumber
        {
            get => Fields[1];
            set => Fields[1] = value;
        }

        #endregion
    }
}