using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookmarksController : ControllerBase
    {
        private readonly ILogger<BookmarksController> _logger;
        private static readonly List<string> _bookmarks = new List<string>();

        public BookmarksController(ILogger<BookmarksController> logger)
        {
            _logger = logger;
        }
        [HttpPost("bookmark")]
        public IActionResult AddBookmark([FromBody] BookmarkRequest request)
        {
            _logger.LogInformation("AddBookmark API called with URL: {url}", request.Url);

            if (string.IsNullOrEmpty(request.Url))
            {
                _logger.LogWarning("Invalid URL: {url}", request.Url);
                return BadRequest("URL не может быть пустым");
            }
            _bookmarks.Add(request.Url);
            _logger.LogInformation("Bookmark added successfully: {url}", request.Url);
            return Ok();
        }
        [HttpGet("checkbookmark")]
        public IActionResult GetBookmarks()
        {
            _logger.LogInformation("GetBookmarks API called");

            return Ok(_bookmarks);
        }
        [HttpDelete("delbookmark")]
        public IActionResult RemoveBookmark([FromQuery] string url)
        {
            _logger.LogInformation("RemoveBookmark API called with URL: {url}", url);

            if (string.IsNullOrEmpty(url))
            {
                _logger.LogWarning("Invalid URL: {url}", url);
                return BadRequest("URL не может быть пустым");
            }

            bool removed = _bookmarks.Remove(url);

            if (removed)
            {
                _logger.LogInformation("Bookmark removed successfully: {url}", url);
                return Ok();
            }
            else
            {
                _logger.LogWarning("Bookmark not found: {url}", url);
                return NotFound();
            }
        }
    }
    public class BookmarkRequest
    {
        public string Url { get; set; }
    }
}