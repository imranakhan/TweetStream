using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TweetStream.Data.Interfaces;

namespace TweetStream.Consumer
{
    /// <summary>
    /// The Consumer Service To Read from the Queue and Write/Save to Data store
    /// Note: This should be a microservice app consuming from a Service Bus queue, it was added as a Background Service library for demo purposes
    /// </summary>
    public class TweetQueueConsumer : BackgroundService
    {
        private readonly int _numOfQueues;
        private readonly ILogger<TweetQueueConsumer> _logger;
        private ConcurrentQueue<string> _queue;
        private readonly ITwitterRepository _twitterRepository;
        public TweetQueueConsumer(ILogger<TweetQueueConsumer> logger, ConcurrentQueue<string> queue, ITwitterRepository twitterRepository, IConfiguration configuration)
        {
            _numOfQueues = configuration.GetValue<int>("NumOfQueues");
            _logger = logger;
            _queue = queue;
            _twitterRepository = twitterRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Create a total of numOfQueues tasks to parallely Consume queue tweets and Save
                List<Task> tasks = new List<Task>();
                for (int i = 1; i <= _numOfQueues; i++)
                {
                    tasks.Add(new Task(async () => await RetrieveAndSave(cancellationToken)));
                }

                // Starting all the tasks in parallel
                Parallel.ForEach(tasks, task =>
                {
                    task.Start();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Queue tweets");
            }
        }

        private async Task RetrieveAndSave(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Retrieve Data from the Concurrent Queue
                    if (_queue.TryDequeue(out string tweet))
                    {
                        Console.WriteLine($"Task:{Task.CurrentId}, Tweet:{tweet}");
                        
                        // update the Repository with the Tweet data
                        _twitterRepository.WriteTweet(tweet);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading tweet from queue");
                }
            }
        }
    }
}