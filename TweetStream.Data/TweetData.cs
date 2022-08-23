using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Data
{
    /// <summary>
    /// Object containing all the required data store's twitter data for calculating statistics
    /// </summary>
    public class TwitterData
    {
        public DateTime? StartTime { get; set; }

        public long TweetCount { get; set; }
        public long ReTweetCount { get; set; }
        public long TweetWithLinksCount { get; set; }
        public Dictionary<string, int> HashTagsDict { get; set; }
    }
}
