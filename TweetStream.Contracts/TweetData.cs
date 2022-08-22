using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStream.Contracts
{
    /// <summary>
    /// Tweet data object for incoming tweet json
    /// </summary>
    public class TweetData
    {
        /// <summary>
        /// The Id of the tweet
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The content of the tweet
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
