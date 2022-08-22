using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TweetStream.Producer.Data.Interfaces;

namespace TweetStream.Producer.Data
{
    /// <summary>
    /// Implementation class for the Twitter Stream reader
    /// </summary>
    public class TwitterStream : IDisposable, ITwitterStream
    {
        private Stream _stream;
        private StreamReader _streamReader;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TwitterStream> _logger;
        private string _url;
        private string _bearerToken;

        public TwitterStream(IHttpClientFactory httpClientFactory, ILogger<TwitterStream> logger, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            var twitterConfig = configuration.GetSection("Twitter");
            _bearerToken = twitterConfig["token"];
            _url = twitterConfig["url"];
            _logger = logger;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");

            _stream = await _httpClient.GetStreamAsync(_url, cancellationToken);
            _streamReader = new StreamReader(_stream, System.Text.Encoding.ASCII, true);
            if (_stream == null || _streamReader == null)
            {
                _logger.LogError("Error creating Stream reader");
            }
        }

        public async Task<string> GetNextAsync() => await _streamReader.ReadLineAsync();

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }
        }
    }
}
