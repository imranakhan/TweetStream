using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TweetStream.Data.Interfaces;
using TweetStream.Service.Interfaces;

namespace TweetStream.Service.Tests
{
    [TestClass]
    public class TwitterServiceUnitTests
    {
        private readonly ITwitterService service;
        private readonly Mock<ITwitterRepository> mockTwitterRepository;
        private readonly Mock<ILogger<TwitterService>> mockLogger;
        public TwitterServiceUnitTests()
        {
            mockTwitterRepository = new Mock<ITwitterRepository>();
            mockLogger = new Mock<ILogger<TwitterService>>();
            service = new TwitterService(mockLogger.Object, mockTwitterRepository.Object);
        }


        #region WriteTweet

        [TestMethod]
        public void TestWriteTweet()
        {
            // Arrange

            // Act

            // Assert

        }

        #endregion WriteTweet

        #region CalculateStatistics

        #endregion CalculateStatistics

        #region RetrieveHashTagsFromTweet

        #endregion RetrieveHashTagsFromTweet
    }
}