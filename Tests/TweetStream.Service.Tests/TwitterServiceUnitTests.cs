using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TweetStream.Contracts;
using TweetStream.Data;
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


        public static TwitterData CreateTwitterDataRecord()
        {
            return new TwitterData
            {
                StartTime = DateTime.Now,
                TweetCount = 100,
                ReTweetCount = 50, 
                TweetWithLinksCount = 5,
                HashTagsDict = new Dictionary<string, int>()
                {
                    { "#ABC", 5 },
                    { "#xyz", 10 },
                    { "#klm", 2 }
                }
            };
        }

        #region WriteTweet

        [TestMethod]
        public void TestWriteTweet_Success_WhenValidTweet()
        {
            // Arrange
            var tweetData = "{\"data\":{\"id\":\"1561783936501440513\",\"text\":\"RT This is a sample Tweet\"}}";
            mockTwitterRepository.Setup(x => x.SaveTweet(It.IsAny<Tweet>())).Verifiable();
            mockTwitterRepository.Setup(x => x.SaveHashTags(It.IsAny<List<string>>())).Verifiable();

            // Act
            service.WriteTweet(tweetData);

            // Assert
            mockTwitterRepository.Verify();
        }

        [TestMethod]
        public void TestWriteTweet_IgnoresAndReturns_WhenInvalidTweet()
        {
            // Arrange
            var invalidTweetData = "{\"data\":null}";
            mockTwitterRepository.Setup(x => x.SaveTweet(It.IsAny<Tweet>())).Verifiable();
            mockTwitterRepository.Setup(x => x.SaveHashTags(It.IsAny<List<string>>())).Verifiable();

            // Act
            service.WriteTweet(invalidTweetData);

            // Assert
            mockTwitterRepository.Verify(repo => repo.SaveTweet(It.IsAny<Tweet>()), Times.Never());
            mockTwitterRepository.Verify(repo => repo.SaveHashTags(It.IsAny<List<string>>()), Times.Never());
        }

        #endregion WriteTweet

        #region CalculateStatistics

        [TestMethod]
        public void TestCalculateStatistics_ReturnsTweetStats_WhenSuccess()
        {
            // Arrange
            var numOfHashtags = 2;
            var expectedStats = CreateTwitterDataRecord();
            mockTwitterRepository.Setup(x => x.GetData()).ReturnsAsync(expectedStats);

            // Act
            var response = service.CalculateStatistics(numOfHashtags)?.Result;

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.TrendingHashTags.Count == numOfHashtags);
            Assert.AreEqual(expectedStats.TweetCount, response.Counts.Tweets);
            Assert.AreEqual(expectedStats.ReTweetCount, response.Counts.ReTweets);
            Assert.AreEqual(expectedStats.TweetWithLinksCount, response.Counts.TweetsWithLink);
        }

        public void TestCalculateStatistics_ReturnsNull_WhenRepository_ThrowsException()
        {
            // Arrange
            var numOfHashtags = 2;
            var expectedStats = CreateTwitterDataRecord();
            mockTwitterRepository.Setup(x => x.GetData()).ThrowsAsync(new Exception("Failed to retrieve twitter data")).Verifiable();

            // Act
            var response = service.CalculateStatistics(numOfHashtags)?.Result;

            // Assert
            Assert.IsNull(response);
        }

        #endregion CalculateStatistics
    }
}