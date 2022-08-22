
namespace TweetStream.Producer.Data.Interfaces
{
    /// <summary>
    /// Interface for the Twitter Stream Reader
    /// </summary>
    public interface ITwitterStream
    {
        Task<string> GetNextAsync();
        Task Initialize(CancellationToken cancellationToken);
    }
}