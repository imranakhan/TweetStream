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
        private int errorCount = 0;

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
            Console.WriteLine("Starting Producer. Please Wait...");
            try
            {
                await _twitterStream.Initialize(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    string tweetStr = await _twitterStream.GetNextAsync();

                    if(!string.IsNullOrWhiteSpace(tweetStr))
                    {
                        _queue.Enqueue(tweetStr);
                    }
                    else if(tweetStr == "")
                    {
                        // TwitterStream appears to occasionally send an empty string "" as the tweet.
                        // Ignoring that and not counting that as a connection issue.
                        _logger.LogInformation("Empty string Tweet received. Skipping");
                    }
                    else
                    {
                        // Possible connection issue with Twitter Stream resulting in null
                        // Backout 0.5 seconds each time and retry
                        Thread.Sleep(500);
                        errorCount++;
                        if (errorCount > 10)
                        {
                            _logger.LogError("Producer: Exceeded error threshold. Exiting!");
                            break;
                        }
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