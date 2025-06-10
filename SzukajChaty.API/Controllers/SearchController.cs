using Microsoft.AspNetCore.Mvc;
using SzukajChaty.API.Models;
using SzukajChaty.API.Services;

namespace SzukajChaty.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ScraperService _scraperService;

    public SearchController(ScraperService scraperService)
    {
        _scraperService = scraperService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Listing>>> Get([FromQuery] SearchCriteria criteria)
    {
        try
        {
            var results = await _scraperService.SearchAsync(criteria);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Błąd serwera: {ex.Message}");
        }
    }
}