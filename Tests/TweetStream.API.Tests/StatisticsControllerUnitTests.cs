using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TweetStream.API.Controllers;
using TweetStream.Contracts;
using TweetStream.Service.Interfaces;

namespace TweetStream.API.Tests
{
    [TestClass]
    public class StatisticsControllerUnitTests
    {
        private readonly StatisticsController controller;
        private readonly Mock<ITwitterService> mockTwitterService;
        private readonly Mock<ILogger<StatisticsController>> mockLogger;
        public StatisticsControllerUnitTests()
        {
            mockTwitterService = new Mock<ITwitterService>();
            mockLogger = new Mock<ILogger<StatisticsController>>();
            controller = new StatisticsController(mockLogger.Object, mockTwitterService.Object);
        }

        public static TweetStats CreateTweetStatsRecord()
        {
            return new TweetStats
            {
                TotalMinutes = 1,
                TotalSeconds = 90,
                TweetsPerSecond = 45,
                Counts = new TweetCount
                {
                    Tweets = 2700,
                    ReTweets = 1500,
                    TweetsWithLink = 50
                },
                TrendingHashTags = new List<TrendingHashTag>
                {
                    new TrendingHashTag 
                    {
                        HashTag = "#BTS",
                        Count = 50
                    },
                    new TrendingHashTag
                    {
                        HashTag = "#abc",
                        Count = 25
                    }
                }
            };
        }

        [TestMethod]
        public void TestGetTweetStatistics()
        {
            // Arrange
            var numOfHasTags = 2;
            var expectedResult = CreateTweetStatsRecord();
            mockTwitterService.Setup(x => x.CalculateStatistics(It.IsAny<int>())).ReturnsAsync(expectedResult);

            // Act
            var response = controller.Get(numOfHasTags).Result;

            // Assert
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsTrue(okResult.StatusCode == 200);
            var result = okResult.Value as TweetStats;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Counts.Tweets == 2700);
            Assert.IsTrue(result.TotalSeconds == 90);
            Assert.IsTrue(result.TrendingHashTags.Count == 2);
        }

        [TestMethod]
        public void TestGetTweetStatistics_500Error()
        {
            // Arrange
            var numOfHasTags = 2;
            var expectedResult = CreateTweetStatsRecord();
            mockTwitterService.Setup(x => x.CalculateStatistics(It.IsAny<int>())).ReturnsAsync((TweetStats)null);

            // Act
            var response = controller.Get(numOfHasTags).Result;

            // Assert
            Assert.IsNull(response.Value);
            var statusCodeResult = response.Result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
        }
    }
}