using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;

namespace TweetStream.Data.Tests
{
    [TestClass]
    public class TwitterRepositoryUnitTests
    {
        private ITwitterRepository repository;
        private readonly Mock<ILogger<TwitterRepository>> mockLogger;
        public TwitterRepositoryUnitTests()
        {
            mockLogger = new Mock<ILogger<TwitterRepository>>();
        }

        public static Tweet CreateNewTweetRecord()
        {
            return new Tweet()
            {
                Data = new TweetData
                {
                    Id = "123213123",
                    Text = "RT This is a test tweet #helloworld"
                }
            };
        }

        public static Tweet CreateNewTweetRecordWithLink()
        {
            return new Tweet()
            {
                Data = new TweetData
                {
                    Id = "12321343",
                    Text = "This is a test tweet https://www.google.com"
                }
            };
        }


        [TestInitialize]
        public void TestInitialize()
        {
            repository = new TwitterRepository(mockLogger.Object);
        }

        #region SaveTweet

        [TestMethod]
        public void TestSaveTweet_WithRT()
        {
            // Arrange
            var tweet = CreateNewTweetRecord();
            

            // Act
            repository.SaveTweet(tweet);

            // Assert
            var stats = repository.GetData().Result;
            Assert.IsTrue(stats.TweetCount == 1);
            Assert.IsTrue(stats.ReTweetCount == 1); // Since the tweet contains the RT string
            Assert.IsTrue(stats.TweetWithLinksCount == 0);
        }

        [TestMethod]
        public void TestSaveTweet_WithLink_WithoutRT()
        {
            // Arrange
            var tweet = CreateNewTweetRecordWithLink();


            // Act
            repository.SaveTweet(tweet);

            // Assert
            var stats = repository.GetData().Result;
            Assert.IsTrue(stats.TweetCount == 1);
            Assert.IsTrue(stats.ReTweetCount == 0); 
            Assert.IsTrue(stats.TweetWithLinksCount == 1); // Since the tweet contains a link
        }

        #endregion SaveTweet

        #region SaveHashTags

        [TestMethod]
        public void TestSaveHashTags_AddsTagsToData()
        {
            // Arrange
            var hashTags = new List<string> { "#helloworld", "#hi" };


            // Act
            repository.SaveHashTags(hashTags);

            // Assert
            var stats = repository.GetData().Result;
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#helloworld"));
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#hi"));
            Assert.IsTrue(stats.HashTagsDict["#helloworld"] == 1);
            Assert.IsTrue(stats.HashTagsDict["#hi"] == 1);
        }

        [TestMethod]
        public void TestSaveHashTags_IncrementsTagsCountInData()
        {
            // Arrange
            var hashTags1 = new List<string> { "#helloworld", "#hi" };
            var hashTags2 = new List<string> { "#helloworld", "#hello" };


            // Act
            repository.SaveHashTags(hashTags1);
            repository.SaveHashTags(hashTags2);

            // Assert
            var stats = repository.GetData().Result;
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#helloworld"));
            Assert.IsTrue(stats.HashTagsDict["#helloworld"] == 2);
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#hi"));
            Assert.IsTrue(stats.HashTagsDict["#hi"] == 1);
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#hello"));
            Assert.IsTrue(stats.HashTagsDict["#hello"] == 1);
        }

        #endregion SaveHashTags

        #region GetData


        [TestMethod]
        public void TestGetData_ReturnsData()
        {
            // Arrange
            var tweet = CreateNewTweetRecord();
            var hashTags1 = new List<string> { "#helloworld", "#hi" };
            var hashTags2 = new List<string> { "#helloworld", "#hello" };
            repository.SaveTweet(tweet);
            repository.SaveHashTags(hashTags1);
            repository.SaveHashTags(hashTags2);

            // Act
            var stats = repository.GetData().Result;

            // Assert
            Assert.IsTrue(stats.TweetCount == 1);
            Assert.IsTrue(stats.ReTweetCount == 1); // Since the tweet contains the RT string
            Assert.IsTrue(stats.TweetWithLinksCount == 0);
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#helloworld"));
            Assert.IsTrue(stats.HashTagsDict["#helloworld"] == 2);
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#hi"));
            Assert.IsTrue(stats.HashTagsDict["#hi"] == 1);
            Assert.IsTrue(stats.HashTagsDict.ContainsKey("#hello"));
            Assert.IsTrue(stats.HashTagsDict["#hello"] == 1);
        }

        #endregion GetData
    }
}