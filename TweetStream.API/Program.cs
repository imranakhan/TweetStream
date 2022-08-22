using System;
using System.Collections.Concurrent;
using TweetStream.Consumer;
using TweetStream.Data;
using TweetStream.Data.Interfaces;
using TweetStream.Producer;
using TweetStream.Producer.Data;
using TweetStream.Producer.Data.Interfaces;
using TweetStream.Service;
using TweetStream.Service.Interfaces;

namespace TweetStream.API
{
    /// <summary>
    /// Main Entry Point of the Application. This currently runs the Producer, Consumer and the Statistics REST API
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<ITwitterRepository, TwitterRepository>();
            builder.Services.AddTransient<ITwitterService, TwitterService>();
            // Actual stream reader for reading the Twitter stream
            builder.Services.AddSingleton<ITwitterStream, TwitterStream>();

            // Ideally should be using ServiceBus queue or RabbitMQ. May swap this out with RabbitMQ in future commits.
            builder.Services.AddSingleton<ConcurrentQueue<string>>();

            builder.Services.AddControllers();

            builder.Services.AddHttpClient();

            // Start a Background service for the Twitter Producer that retrieves tweets from the Stream and Queues them
            // This should ideally be its own separate microservice app that pushes to a queue.
            builder.Services.AddHostedService<TweetQueueProducer>();

            // Start a Background Service for the Tweet Consumer that pulls from the shared queue and processes them
            // This should ideally be its own separate microservice app that pulls from the queue and writes to the database
            builder.Services.AddHostedService<TweetQueueConsumer>();

            // Configuring Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}