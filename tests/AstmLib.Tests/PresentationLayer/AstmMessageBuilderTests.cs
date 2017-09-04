using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstmLib.Configuration;
using AstmLib.PresentationLayer;
using Shouldly;
using Xunit;

namespace BaseLib.Astm.Tests
{
    public class AstmMessageBuilderTests
    {
        [Fact]
        public void DoubleQueryRecordTest()
        {
            List<string> records = new List<string>
            {
                "H|\\^&\r",
                "Q|1|123abc|^^^ALL\r",
                "L|1\r"
            };

            AstmMessage[] msg = AstmMessageBuilder.Build(records.ToArray(), new AstmHighLevelSettings());
            msg.Length.ShouldBe(1);
            msg[0].HeaderRecord.Children.Length.ShouldBe(1);
            ((AstmQueryRecord)msg[0].HeaderRecord.Children.First()).StartingRangeId.ShouldBe("123abc");
        }
    }
}
