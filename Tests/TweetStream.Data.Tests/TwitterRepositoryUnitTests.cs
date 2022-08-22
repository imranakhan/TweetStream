using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TweetStream.Data.Interfaces;

namespace TweetStream.Data.Tests
{
    [TestClass]
    public class TwitterRepositoryUnitTests
    {
        private readonly ITwitterRepository repository;
        private readonly Mock<ILogger<TwitterRepository>> mockLogger;
        public TwitterRepositoryUnitTests()
        {
            mockLogger = new Mock<ILogger<TwitterRepository>>();
            repository = new TwitterRepository(mockLogger.Object);
        }

        #region SaveTweet

        [TestMethod]
        public void TestSaveTweet()
        {
            // Arrange

            // Act

            // Assert
        }

        #endregion SaveTweet

        #region SaveHashTags

        #endregion SaveHashTags

        #region GetData

        #endregion GetData
    }
}