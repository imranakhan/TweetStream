using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Contracts
{
    public class TweetCount
    {
        public long Tweets { get; set; }

        public long ReTweets { get; set; }

        public long TweetsWithLink { get; set; }
    }
}
