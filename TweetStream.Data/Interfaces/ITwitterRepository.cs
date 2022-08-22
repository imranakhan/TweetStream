using TweetStream.Contracts;

namespace TweetStream.Data.Interfaces
{
    public interface ITwitterRepository
    {
        Task<TweetStats> GetStatistics(int numOfHashTags = 10);
        void WriteTweet(string tweetData);
    }
}