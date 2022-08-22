using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;
using TweetStream.Service.Interfaces;

namespace TweetStream.Service
{
    public class TwitterService : ITwitterService
    {
        private ILogger<TwitterService> _logger;
        private readonly ITwitterRepository _twitterRepository;

        public TwitterService(ILogger<TwitterService> logger, ITwitterRepository twitterRepository)
        {
            _logger = logger;
            _twitterRepository = twitterRepository;
        }

        /// <summary>
        /// Write the tweet data to the repository
        /// </summary>
        /// <param name="tweetData"></param>
        public async void WriteTweet(string tweetData)
        {
            if (tweetData == null)
            {
                _logger.LogWarning($"Invalid tweet {tweetData} received. Ignoring and continuing");
                return;
            }

            Tweet tweet = JsonConvert.DeserializeObject<Tweet>(tweetData);

            if (tweet == null || tweet.Data == null || tweet.Data.Text == null)
            {
                // Invalid Tweet, Log and Return
                _logger.LogWarning($"Invalid tweet {tweet} received. Ignoring and continuing");
                return;
            }

            try
            {
                // Persist Tweet
                _twitterRepository.SaveTweet(tweet);

                // Retrieve HashTags in the tweet (if any)
                var hashTags = RetrieveHashTagsFromTweet(tweet.Data.Text);

                // Save HashTags in Tweet
                _twitterRepository.SaveHashTags(hashTags);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Saving Tweet and HasTags");
            }
        }

        /// <summary>
        /// Caluclate the Statistics of the tweets received and saved so far
        /// </summary>
        /// <param name="numOfHashTags">Number of Top Trending Hashtags to return</param>
        /// <returns>TweetStats object containing the statistics</returns>
        public async Task<TweetStats> CalculateStatistics(int numOfHashTags = 10)
        {
            TweetStats tweetStats = null;
            try
            {
                // Ready Database asynchronously from database
                var data = await _twitterRepository.GetData();

                // Calculate statistics
                long totalSeconds = (long)DateTime.Now.Subtract(data.StartTime.Value).TotalSeconds;
                long totalMinutes = (long)DateTime.Now.Subtract(data.StartTime.Value).TotalMinutes;
                long tweetsPerSec = totalSeconds == 0 ? 0 : data.TweetCount / totalSeconds;

                var numOfTags = numOfHashTags > 0 ? numOfHashTags : 10;
                var orderedDictionary = data.HashTagsDict
                                            .OrderByDescending(x => x.Value)
                                            .ToDictionary(x => x.Key, x => x.Value);

                // Retrieve the given number of most trending HashTags
                List<TrendingHashTag>? trendingHashTags = null;
                if (orderedDictionary != null)
                {
                    trendingHashTags = orderedDictionary
                                        .Take(numOfTags)
                                        .Select(x => { return new TrendingHashTag { HashTag = x.Key, Count = x.Value }; })?.ToList();
                }

                tweetStats = new TweetStats()
                {
                    TotalSeconds = totalSeconds,
                    TotalMinutes = totalMinutes,
                    TweetsPerSecond = tweetsPerSec,
                    Counts = new TweetCount
                    {
                        Tweets = data.TweetCount,
                        ReTweets = data.ReTweetCount,
                        TweetsWithLink = data.TweetWithLinksCount,
                    },
                    TrendingHashTags = trendingHashTags

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Retrieving Tweet statistics");
            }

            return tweetStats;
        }

        /// <summary>
        /// Retrieve the list of hashtags from a given tweet content
        /// </summary>
        /// <param name="tweet">The tweet's content</param>
        /// <returns>List of strings representing the hashtags found in the given tweet</returns>
        public List<string> RetrieveHashTagsFromTweet(string tweet)
        {
            var hashTagPattern = $"(^|\\s)#([A-Za-z_][A-Za-z0-9_]*)";
            if (string.IsNullOrEmpty(tweet))
            {
                return new List<string>();
            }
            var regEx = new Regex(hashTagPattern, RegexOptions.IgnoreCase);
            var matches = regEx.Matches(tweet).Cast<Match>().Select(x => x.Value);
            return matches?.ToList();
        }
    }
}