using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Contracts
{
    /// <summary>
    /// Response Statistics object that contains all the various counts and hastags needed
    /// </summary>
    public class TweetStats
    {
        /// <summary>
        /// Total Minutes the Tweet store has been saving so far
        /// </summary>
        public long TotalMinutes { get; set; }

        /// <summary>
        /// Total Seconds for which the Tweet Store has been saving so far 
        /// </summary>
        public long TotalSeconds { get; set; }

        /// <summary>
        /// Frequency of Tweets emitted by the Stream
        /// </summary>
        public long TweetsPerSecond { get; set; }

        /// <summary>
        /// Counts of the Tweets received so far. This includes (Tweets, Re-Tweets and Tweets with Links)
        /// </summary>
        public TweetCount Counts { get; set; }

        /// <summary>
        /// List of the top Trending HasTags from the Sample set
        /// </summary>
        public List<TrendingHashTag> TrendingHashTags { get; set; }

    }
}
