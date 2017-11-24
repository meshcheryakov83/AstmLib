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
        public void Query_should_be_parsed_successfully()
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

        [Fact]
        public void Multi_result_message__should_be_parsed_successfully()
        {
            string message =
                "H|\\^&|||Helena Laboratories^HemoRAM^Version 1.3^SN 631715802|1530 Lindbergh Dr., Beaumont, TX. 77704||409-842-3714||||P|1394-97|20171121132305\r" +
"P|1|4545067|||||||||||||||||||||||||||||||||\r" +
"O|1||122|^^^Platelet Aggregation^*Screen 1^Adenosine Diphosphate^Patient^^0^^0,00^50,00^uM|||||||||||||||||||||F\r" +
"R|1|^^^^^^^^^^^^^InitialMax|0,055||||||||20171120171146||\r" +
"R|2|^^^^^^^^^^^^^InitialMin|0,764||||||||20171120171146||\r" +
"R|3|^^^^^^^^^^^^^MaxPercent|64,5|%|||||||20171120171146||\r" +
"R|4|^^^^^^^^^^^^^MaxPCTime|152|sec|||||||20171120171146||\r" +
"R|5|^^^^^^^^^^^^^Slope|116,7||||||||20171120171146||\r" +
"R|6|^^^^^^^^^^^^^LagTime|5,2|sec|||||||20171120171146||\r" +
"L|1|N";
            
            AstmMessage[] msg = AstmMessageBuilder.Build(message.Split('\r'), new AstmHighLevelSettings());
            msg.Length.ShouldBe(1);
            msg[0].HeaderRecord.Patients.First().Orders.First().Children.Length.ShouldBe(6);
        }
    }
}
