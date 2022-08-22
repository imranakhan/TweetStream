using Microsoft.AspNetCore.Mvc;
using TweetStream.Contracts;
using TweetStream.Data.Interfaces;

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
        private readonly ITwitterRepository _twitterRepository;

        public StatisticsController(ILogger<StatisticsController> logger, ITwitterRepository twitterRepository)
        {
            _logger = logger;
            _twitterRepository = twitterRepository;
        }

        [HttpGet(Name = "GetTweetStatistics")]
        public async Task<ActionResult<TweetStats>> Get(int numHashTags)
        {
            return await _twitterRepository.GetStatistics(numHashTags);
        }
    }
}