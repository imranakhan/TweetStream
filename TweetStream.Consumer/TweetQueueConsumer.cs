﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TweetStream.Service.Interfaces;

namespace TweetStream.Consumer
{
    /// <summary>
    /// The Consumer Service To Read from the Queue and Write/Save to Data store
    /// Note: This should be a microservice app consuming from a Service Bus queue, it was added as a Background Service library for demo purposes as we are using in-memory Singleton data store
    /// </summary>
    public class TweetQueueConsumer : BackgroundService
    {
        private readonly int _numOfQueues;
        private readonly ILogger<TweetQueueConsumer> _logger;
        private ConcurrentQueue<string> _queue;
        private readonly ITwitterService _twitterService;
        public TweetQueueConsumer(ILogger<TweetQueueConsumer> logger, ConcurrentQueue<string> queue, ITwitterService twitterService, IConfiguration configuration)
        {
            _numOfQueues = configuration.GetValue<int>("NumOfQueues");
            _logger = logger;
            _queue = queue;
            _twitterService = twitterService;
        }

        /// <summary>
        /// Main execute method that runs on Start of the Background Service Worker
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieve the tweet from the Queue and write it using the service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
                        _twitterService.WriteTweet(tweet);
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