using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Contracts
{
    /// <summary>
    /// Total Count of tweets of each Type
    /// </summary>
    public class TweetCount
    {
        /// <summary>
        /// Total number of Tweets
        /// </summary>
        public long Tweets { get; set; }

        /// <summary>
        /// Total number of tweets that are Re-Tweets
        /// </summary>
        public long ReTweets { get; set; }

        /// <summary>
        /// Total number of tweets that contain one or more links
        /// </summary>
        public long TweetsWithLink { get; set; }
    }
}
