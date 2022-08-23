using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TweetStream.Producer.Data.Interfaces;

namespace TweetStream.Producer.Tests
{
    [TestClass]
    public class TweetQueueProducerUnitTests
    {
        private readonly Mock<ILogger<TweetQueueProducer>> mockLogger;
        private ConcurrentQueue<string> queue;
        private Mock<ITwitterStream> mockTwitterStream;
        private TweetQueueProducer producer;
        private IHostedService hostedService;

        private string testTweet = "{\"data\":{\"id\":\"23423423\",\"text\":\"This is a Test Tweet\"}}";

        public TweetQueueProducerUnitTests()
        {
            mockTwitterStream = new Mock<ITwitterStream>();
            mockLogger = new Mock<ILogger<TweetQueueProducer>>();
            queue = new ConcurrentQueue<string>();
            producer = new TweetQueueProducer(mockLogger.Object, queue, mockTwitterStream.Object);
        }

        [TestMethod]
        public void TestProducer_AddingToQueue()
        {
            // Arrange
            CancellationToken cancellationToken = new CancellationToken();
            mockTwitterStream.Setup(x => x.Initialize(It.IsAny<CancellationToken>())).Verifiable();

            mockTwitterStream.Setup(x => x.GetNextAsync().Result).Returns(() =>
            {
                return (queue.Count > 0 ? null : testTweet); // To return the tweet only the first time, and null afterwards in the test
            });

            producer.StartAsync(cancellationToken).ConfigureAwait(false);

            // Act
            Thread.Sleep(500);  // Waiting so it queues up the test Tweet
            bool hasItem = queue.TryDequeue(out string resultTweet);

            producer.StopAsync(cancellationToken);

            // Assert
            mockTwitterStream.Verify();
            Assert.IsTrue(hasItem);
            Assert.AreEqual(testTweet, resultTweet);
        }
    }
}