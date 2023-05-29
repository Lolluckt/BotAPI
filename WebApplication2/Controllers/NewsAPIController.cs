using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Diagnostics.Metrics;
using MyApplication.Helpers;


namespace MyApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly string apiKey = "b868441aa9a14288a07b222261ba1657";
        private readonly ILogger<NewsController> _logger;
        private readonly HttpClient _client;
        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet("querry")]
        public IActionResult GetNews([FromQuery] string q)
        {
            _logger.LogInformation("GetNews API called with query: {query}", q);

            if (string.IsNullOrEmpty(q))
            {
                _logger.LogWarning("Invalid query: {query}", q);
                return BadRequest("Параметр 'q' не может быть пустым");
            }

            var newsApiClient = new NewsApiClient(apiKey);

            var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            {
                Q = q,
                Language = Languages.EN,
                PageSize = 3
            });

            if (articlesResponse.Status == Statuses.Ok)
            {
                _logger.LogInformation("News retrieved successfully for query: {query}", q);
                return Ok(articlesResponse.Articles);
            }
            else
            {
                _logger.LogError("Failed to retrieve news for query: {query}. Error: {errorMessage}", q, articlesResponse.Error.Message);
                return BadRequest("Ошибка при выполнении запроса: " + articlesResponse.Error.Message);
            }
        }
        [HttpGet("country")]
        public IActionResult GetNewsByCountry([FromQuery] string countryCode)
        {
            _logger.LogInformation("GetNewsByCountry API called with country: {country}", countryCode);

            if (string.IsNullOrEmpty(countryCode))
            {
                _logger.LogWarning("Invalid country: {country}", countryCode);
                return BadRequest("Параметр 'country' не может быть пустым");
            }

            var country = CountryConverter.ConvertToCountry(countryCode);

            if (country == null)
            {
                _logger.LogWarning("Invalid country code: {countryCode}", countryCode);
                return BadRequest("Неверный код страны");
            }

            var newsApiClient = new NewsApiClient(apiKey);

            var articlesResponse = newsApiClient.GetTopHeadlines(new TopHeadlinesRequest
            {
                Country = country.Value,
                PageSize = 3
            });

            if (articlesResponse.Status == Statuses.Ok)
            {
                _logger.LogInformation("News retrieved successfully for country: {country}", country);
                return Ok(articlesResponse.Articles);
            }
            else
            {
                _logger.LogError("Failed to retrieve news for country: {country}. Error: {errorMessage}", country, articlesResponse.Error.Message);

                return BadRequest("Ошибка при выполнении запроса: " + articlesResponse.Error.Message);
            }
        }
        [HttpPost("NewsURL")]
        public async Task<IActionResult> ExtractData([FromBody] ExtractRequest request)
        {
            _logger.LogInformation("ExtractData API called with URL: {url}", request.Url);

            if (string.IsNullOrEmpty(request.Url))
            {
                _logger.LogWarning("Invalid URL: {url}", request.Url);
                return BadRequest("URL не может быть пустым");
            }

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "5f4e4fdb5bmsh1467d5f260a96dep12da58jsn105fe83bc7ea");
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "news-article-data-extract-and-summarization1.p.rapidapi.com");

            var requestBody = new { url = request.Url };

            try
            {
                var response = await _client.PostAsJsonAsync("https://news-article-data-extract-and-summarization1.p.rapidapi.com/extract/", requestBody);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to extract data for URL: {url}. Error: {errorMessage}", request.Url, ex.Message);
                return BadRequest("Ошибка при выполнении запроса: " + ex.Message);
            }
        }
        [HttpGet("location")]
        public async Task<IActionResult> GetNewsByLocation()
        {
            var ipAddress = await Location.GetIpAddressAsync();
            var location = await Location.GetUserLocationAsync(ipAddress);

            if (location != null)
            {
                var city = location.Value<string>("city");
                if (string.IsNullOrEmpty(city))
                {
                    _logger.LogWarning("Invalid city: {city}");
                    return BadRequest("Не удалось определить город пользователя");
                }

                var newsApiClient = new NewsApiClient(apiKey);

                var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
                {
                    Q = city,
                    Language = Languages.EN,
                    PageSize = 2
                });

                if (articlesResponse.Status == Statuses.Ok)
                {
                    _logger.LogInformation("News retrieved successfully for city: {city}", city);
                    return Ok(new
                    {
                        Location = city,
                        News = articlesResponse.Articles
                    });
                }
                else
                {
                    _logger.LogError("Failed to retrieve news for city: {city}. Error: {errorMessage}", city, articlesResponse.Error.Message);
                    return BadRequest("Ошибка при выполнении запроса: " + articlesResponse.Error.Message);
                }
            }
            else
            {
                _logger.LogWarning("Failed to get user location");
                return BadRequest("Не удалось определить местоположение пользователя");
            }
        }

    }

    public class ExtractRequest
    {
        public string Url { get; set; }
    }


}


