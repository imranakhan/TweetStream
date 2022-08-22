using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Contracts
{
    public class TrendingHashTag
    {
        /// <summary>
        /// The Hastag string
        /// </summary>
        public string HashTag { get; set; }

        /// <summary>
        /// The Total number of occurences so far
        /// </summary>
        public long Count { get; set; }
    }
}
