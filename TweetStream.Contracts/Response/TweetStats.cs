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
        public long TotalMinutes { get; set; }

        public long TotalSeconds { get; set; }
        public long TweetsPerSecond { get; set; }
        public TweetCount Counts { get; set; }

        public List<TrendingHashTag> TrendingHashTags { get; set; }

    }
}
