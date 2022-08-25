using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;

namespace TweetStream.Data
{
    /// <summary>
    /// The main Twitter Repository that Saves and retrieves data from the Data Store
    /// </summary>
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

        /// <summary>
        /// Save the tweet data and update counts from an incoming Tweet into the data store
        /// </summary>
        /// <param name="tweet"></param>
        public void SaveTweet(Tweet tweet)
        {
            // Set StartDate on first receving writter data
            if (startTime == null)
            {
                startTime = DateTime.Now;
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
        }

        /// <summary>
        /// Saves the list of HasTags into the hasTags dictionary with their counts for ease of statistics
        /// </summary>
        /// <param name="hashTags"></param>
        public void SaveHashTags(List<string> hashTags)
        {
            // Add HashTags to dictionary with their count so far to determine most used tags
            if (hashTags != null && hashTags.Count > 0)
            {
                lock (this)
                {
                    foreach (var hashTag in hashTags)
                    {
                        var tempHashTag = hashTag.Trim();
                        if (hashTagsDict.TryGetValue(tempHashTag, out int count))
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

        /// <summary>
        /// Retrieve the Tweet data required for Statistics
        /// </summary>
        /// <returns></returns>
        public async Task<TwitterData> GetData()
        {
            // Ready Database asynchronously here if we have a database
            return new TwitterData
            {
                StartTime = startTime,
                TweetCount = tweetCount,
                ReTweetCount = retweetCount,
                TweetWithLinksCount = tweetWithLinkCount,
                HashTagsDict = hashTagsDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}