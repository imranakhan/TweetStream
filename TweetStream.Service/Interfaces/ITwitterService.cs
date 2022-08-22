using TweetStream.Contracts;

namespace TweetStream.Service.Interfaces
{
    /// <summary>
    /// Interface for the TwitterService
    /// </summary>
    public interface ITwitterService
    {
        Task<TweetStats> CalculateStatistics(int numOfHashTags);
        void WriteTweet(string tweetData);
    }
}