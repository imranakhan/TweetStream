using TweetStream.Contracts;

namespace TweetStream.Data.Interfaces
{
    /// <summary>
    /// Interface for the Twitter Repository
    /// </summary>
    public interface ITwitterRepository
    {
        void SaveTweet(Tweet tweet);
        void SaveHashTags(List<string> hashTags);
        Task<TweetData> GetData();
    }
}