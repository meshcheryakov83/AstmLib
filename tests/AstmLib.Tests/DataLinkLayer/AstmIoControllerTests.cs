using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using AstmLib.Configuration;
using AstmLib.DataLinkLayer;
using Microsoft.Extensions.Logging;

namespace AstmLib.Tests.DataLinkLayer
{
    public class AstmIOControllerTests
    {
        [Fact]
        public void IntegrationTest()
        {
            string uploadedMessage = null;
            string downloadedMessage = null;
            Exception uploadException = null;
            Exception downloadException = null;
            string message = @"tests\r\n"
                             + "esttesttesttesttesttesttestesttesttesttesttesttesttesttestesttesttesttestt"
                             + "esttesttesttestesttesttesttesttesttesttesttestesttesttesttesttesttesttesttestesttes"
                             + "ttesttesttesttesttesttestesttesttesttesttesttesttesttestesttesttesttestte"
                             + "sttesttesttestesttesttesttesttesttesttesttestesttesttesttesttesttesttesttest\r\n";
            var loggingFactory = new LoggerFactory();
            loggingFactory.AddDebug();

            Queue<byte> buf1 = new Queue<byte>();
            Queue<byte> buf2 = new Queue<byte>();

            var taskListener = Task.Run(async () =>
            {
                var stream = new InMemotyChannel(buf1, buf2);
                var lowLevelSettings = new AstmLowLevelSettings();

                var controller = new AstmIOController(stream, lowLevelSettings, loggingFactory);

                controller.Start();
                controller.AddMessageToUploadQueue(message);
                controller.MessageUploadCompleted += (sender, args) =>
                {
                    controller.Stop();
                    uploadedMessage = args.Message;
                    uploadException = args.OccuredException;
                };
                while (controller.IsRunning)
                {
                    await Task.Delay(100);
                }
            });

            var taskClient = Task.Run(async () =>
            {
                var stream = new InMemotyChannel(buf2, buf1);
                var lowLevelSettings = new AstmLowLevelSettings();

                var controller = new AstmIOController(stream, lowLevelSettings, loggingFactory);

                controller.Start();
                controller.MessageDownloadCompleted += (sender, args) =>
                {
                    downloadedMessage = args.Message;
                    downloadException = args.OccuredException;
                    controller.Stop();
                };
                while (controller.IsRunning)
                {
                    await Task.Delay(100);
                }
            });

            Task.WaitAll(taskClient, taskListener);

            uploadException.ShouldBeNull();
            downloadException.ShouldBeNull();
            uploadedMessage.ShouldBe(message);
            downloadedMessage.ShouldBe(message);
        }
    }
}
