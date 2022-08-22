using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;

namespace TweetStream.Data
{
    public class TwitterRepository : ITwitterRepository
    {
        private ConcurrentBag<Tweet> list;
        private ConcurrentDictionary<string, int> hashTagsDict;
        private ILogger<TwitterRepository> _logger;
        private DateTime? startTime = null;
        private long tweetCount = 0;
        private long retweetCount = 0;
        private long tweetWithLinkCount = 0;

        public TwitterRepository(ILogger<TwitterRepository> logger)
        {
            _logger = logger;
            list = new ConcurrentBag<Tweet>();
            hashTagsDict = new ConcurrentDictionary<string, int>();
        }

        public void WriteTweet(string tweetData)
        {
            if (tweetData == null)
            {
                _logger.LogWarning($"Invalid tweet {tweetData} received. Ignoring and continuing");
                return;
            }

            // set startdate on first receving writter data
            if (startTime == null)
            {
                startTime = DateTime.Now;
            }

            Tweet tweet = JsonConvert.DeserializeObject<Tweet>(tweetData);

            if (tweet == null || tweet.Data == null || tweet.Data.Text == null)
            {
                // Invalid Tweet, Log and Return
                _logger.LogWarning($"Invalid tweet {tweet} received. Ignoring and continuing");
                return;
            }

            // Instead of add to a concurrent bag of objects, you write this to a database here
            list.Add(tweet);

            // Increment the tweetCounter
            Interlocked.Increment(ref tweetCount);

            // Check and Increment the retweetCounter
            if (tweet.Data.Text.StartsWith("RT"))
            {
                Interlocked.Increment(ref retweetCount);
            }

            //Check and Increment the tweet with Links counter
            if (tweet.Data.Text.Contains("https://") || tweet.Data.Text.Contains("http://"))
            {
                Interlocked.Increment(ref tweetWithLinkCount);
            }

            // Retrieve HashTags in the tweet (if any)
            var hashTags = GetHashTags(tweet.Data.Text);

            // Add HashTags to dictionary with their count so far to determine most used tags
            if (hashTags != null && hashTags.Count > 0)
            {
                lock (this)
                {
                    foreach (var hashTag in hashTags)
                    {
                        var tempHashTag = hashTag.Trim();
                        if (hashTagsDict.ContainsKey(tempHashTag)
                            && hashTagsDict.TryGetValue(tempHashTag, out int count))
                        {
                            hashTagsDict[tempHashTag] = count + 1;
                        }
                        else
                        {
                            hashTagsDict[tempHashTag] = 1;
                        }
                    }
                }
            }
        }

        public List<string> GetHashTags(string tweet)
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

        public async Task<TweetStats> GetStatistics(int numOfHashTags = 10)
        {

            // Ready Database asynchronously here if we have a database

            // Calculate statistics
            long totalSeconds = (long)DateTime.Now.Subtract(startTime.Value).TotalSeconds;
            long totalMinutes = (long)DateTime.Now.Subtract(startTime.Value).TotalMinutes;
            long tweetsPerSec = totalSeconds == 0 ? 0 : tweetCount / totalSeconds;

            var orderedDictionary = hashTagsDict
                                        .OrderByDescending(x => x.Value)
                                        .ToDictionary(x => x.Key, x => x.Value);
            var numOfTags = numOfHashTags > 0 ? numOfHashTags : 10;

            // Retrieve the given number of most trending HashTags
            List<TrendingHashTag>? trendingHashTags = null;
            if (orderedDictionary != null)
            {
                trendingHashTags = orderedDictionary
                                    .Take(numOfTags)
                                    .Select(x => { return new TrendingHashTag { HashTag = x.Key, Count = x.Value }; })?.ToList();
            }

            TweetStats tweetStats = new TweetStats()
            {
                TotalSeconds = totalSeconds,
                TotalMinutes = totalMinutes,
                TweetsPerSecond = tweetsPerSec,
                Counts = new TweetCount
                {
                    Tweets = tweetCount,
                    ReTweets = retweetCount,
                    TweetsWithLink = tweetWithLinkCount,
                },
                TrendingHashTags = trendingHashTags

            };
            return tweetStats;
        }
    }
}