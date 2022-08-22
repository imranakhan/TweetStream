using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Data
{
    /// <summary>
    /// Object containing all the required data store's tweet data for calculating statistics
    /// </summary>
    public class TweetData
    {
        public DateTime? StartTime { get; set; }

        public long TweetCount { get; set; }
        public long ReTweetCount { get; set; }
        public long TweetWithLinksCount { get; set; }
        public Dictionary<string, int> HashTagsDict { get; set; }
    }
}
