using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using System.Diagnostics.Metrics;
using MyApplication.Helpers;
using Npgsql;

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
                SortBy = SortBys.Popularity,
                Language = Languages.EN,
                From = new DateTime(2023, 5, 25),
                PageSize = 2
               
            });
            if (articlesResponse.Status == Statuses.Ok)
            {
                _logger.LogInformation("News retrieved successfully for query: {query}", q);

                // Добавление записей в базу данных
                using (var connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=newsdb;Username=postgres;Password=123456"))
                {
                    connection.Open();
                    using (AppDbContext db = new AppDbContext())
                    {
                        List<Articles> articlesToAdd = new List<Articles>();

                        foreach (var article in articlesResponse.Articles)
                        {
                            Articles article1 = new Articles { Title = article.Title, Description = article.Description };
                            articlesToAdd.Add(article1);
                        }

                        db.Articles.AddRange(articlesToAdd);
                        db.SaveChanges();
                    }
                }
            
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
                _logger.LogInformation("News retrieved successfully for query: {query}", countryCode);

                // Добавление записей в базу данных
                using (var connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=newsdb;Username=postgres;Password=123456"))
                {
                    connection.Open();

                    using (AppDbContext db = new AppDbContext())
                    {
                        List<Articles2> articlesToAdd = new List<Articles2>();

                        foreach (var article in articlesResponse.Articles)
                        {
                            Articles2 article1 = new Articles2 { Title = article.Title, Description = article.Description, Country = countryCode};
                            articlesToAdd.Add(article1);
                        }

                        db.Articles2.AddRange(articlesToAdd);
                        db.SaveChanges();
                    }
                }

                return Ok(articlesResponse.Articles);
            }
            else
            {
                _logger.LogError("Failed to retrieve news for query: {query}. Error: {errorMessage}", countryCode, articlesResponse.Error.Message);
                return BadRequest("Ошибка при выполнении запроса: " + articlesResponse.Error.Message);
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
                    SortBy = SortBys.Relevancy,
                    Language = Languages.EN,
                    PageSize = 1
                });

                if (articlesResponse.Status == Statuses.Ok)
                {
                    _logger.LogInformation("News retrieved successfully for query: {query}", city);

                    using (var connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=newsdb;Username=postgres;Password=123456"))
                    {
                        connection.Open();

                        using (AppDbContext db = new AppDbContext())
                        {
                            List<Articles3> articlesToAdd = new List<Articles3>();

                            foreach (var article in articlesResponse.Articles)
                            {
                                Articles3 article1 = new Articles3 { Title = article.Title, Description = article.Description, Location = city};
                                articlesToAdd.Add(article1);
                            }

                            db.Articles3.AddRange(articlesToAdd);
                            db.SaveChanges();
                        }
                    }

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
}


