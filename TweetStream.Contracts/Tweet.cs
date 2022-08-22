using Newtonsoft.Json;

namespace TweetStream.Contracts
{
    /// <summary>
    /// Incoming Tweet object
    /// </summary>
    public class Tweet
    {
        /// <summary>
        /// The incoming Tweet data object of a tweet
        /// </summary>
        [JsonProperty("data")]
        public TweetData Data { get; set; }
    }
}