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
        public string HashTag { get; set; }

        public long Count { get; set; }
    }
}
