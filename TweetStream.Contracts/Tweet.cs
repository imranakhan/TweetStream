using Newtonsoft.Json;

namespace TweetStream.Contracts
{
    public class Tweet
    {
        [JsonProperty("data")]
        public TweetData Data { get; set; }
    }
}