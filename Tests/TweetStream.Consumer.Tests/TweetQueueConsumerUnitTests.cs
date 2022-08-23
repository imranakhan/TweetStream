using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TweetStream.Service.Interfaces;

namespace TweetStream.Consumer.Tests
{
    [TestClass]
    public class TweetQueueConsumerUnitTests
    {
        Mock<ILogger<TweetQueueConsumer>> mockLogger;
        private ConcurrentQueue<string> queue;
        Mock<ITwitterService> mockTwitterService;
        private TweetQueueConsumer consumer;

        private string testTweet = "{\"data\":{\"id\":\"23423423\",\"text\":\"This is a Test Tweet\"}}";

        public TweetQueueConsumerUnitTests()
        {
            mockLogger = new Mock<ILogger<TweetQueueConsumer>>();
            queue = new ConcurrentQueue<string>();
            mockTwitterService = new Mock<ITwitterService>();
            var myConfiguration = new Dictionary<string, string> { { "NumOfConsumers", "1"} };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            consumer = new TweetQueueConsumer(mockLogger.Object, queue, mockTwitterService.Object, configuration);
        }

        [TestMethod]
        public void TestQueueConsumer_Read()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken();
            mockTwitterService.Setup(x => x.WriteTweet(It.IsAny<string>())).Verifiable();

            consumer.StartAsync(cancellationToken).ConfigureAwait(false);

            // Act
            queue.Enqueue(testTweet);
            Thread.Sleep(500);  //Introduce a delay so the Tweet can be consumed before the service is stopped

            // Assert
            consumer.StopAsync(cancellationToken);
            mockTwitterService.Verify(service => service.WriteTweet(testTweet), Times.Once());
        }
    }
}