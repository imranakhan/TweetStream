using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TweetStream.Producer.Data;
using TweetStream.Producer.Data.Interfaces;

namespace TweetStream.Producer
{
    /// <summary>
    /// The Producer Service to Read from a Twitter Stream and write to the queue
    /// Note: This should be a microservice app publishing to a Service Bus queue, it was added as a Background Service library for demo purposes as we are using in-memory Singleton data store
    /// </summary>
    public class TweetQueueProducer : BackgroundService
    {
        private readonly ILogger<TweetQueueProducer> _logger;
        private ConcurrentQueue<string> _queue;
        private ITwitterStream _twitterStream;

        public TweetQueueProducer(ILogger<TweetQueueProducer> logger, ConcurrentQueue<string> queue, ITwitterStream twitterStream)
        {
            _logger = logger;
            _twitterStream = twitterStream;
            _queue = queue;
        }

        /// <summary>
        /// Main execute method that runs on Start of the Background Service Worker
        /// This intializes the twitter stream, loops to Retrieves the next Tweet and adds it to the Queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _twitterStream.Initialize(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    string tweetStr = await _twitterStream.GetNextAsync();

                    if(tweetStr != null)
                    {
                        _queue.Enqueue(tweetStr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading Twitter stream");
            }
        }
    }
}