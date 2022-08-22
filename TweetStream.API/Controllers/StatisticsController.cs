using Microsoft.AspNetCore.Mvc;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;
using TweetStream.Service.Interfaces;

namespace TweetStream.API.Controllers
{
    /// <summary>
    /// Main Entry point API controller for Retrieving Tweet Statistics when Streaming
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly ITwitterService _twitterService;

        public StatisticsController(ILogger<StatisticsController> logger, ITwitterService twitterService)
        {
            _logger = logger;
            _twitterService = twitterService;
        }

        /// <summary>
        /// Get Tweet Statistics
        /// </summary>
        /// <param name="numHashTags">Number of Trending HashTags to return</param>
        /// <returns>The Tweet Statistics so far</returns>
        [HttpGet(Name = "GetTweetStatistics")]
        public async Task<ActionResult<TweetStats>> Get(int numHashTags)
        {
            return await _twitterService.CalculateStatistics(numHashTags);
        }
    }
}