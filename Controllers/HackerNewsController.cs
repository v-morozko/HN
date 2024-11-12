using HackerNews.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Controllers;

[ApiController]
[Route("[controller]")]
public class HackerNewsController(IHackerNewsService hackerNewsService)
    : ControllerBase
{
    [Route("BestStories")]
    [HttpGet]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<IActionResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<IActionResult>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] int count, CancellationToken cancellationToken = default)
    {
        var items = await hackerNewsService.GetBestStoriesAsync(count, cancellationToken);
        if (items != null)
        {
            return new ContentResult
            {
                Content = $"[{string.Join(", ", items)}]",
                ContentType = "application/json"
            };
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}