using SzukajChaty.API.Models;

namespace SzukajChaty.API.Services;

public class ScraperService
{
    private readonly IEnumerable<IScraper> _scrapers;

    public ScraperService(IEnumerable<IScraper> scrapers)
    {
        _scrapers = scrapers;
    }

    public async Task<List<Listing>> SearchAsync(SearchCriteria criteria)
    {
        var activeScrapers = _scrapers
            .Where(s => criteria.Portals.Contains(s.SourceName, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var tasks = activeScrapers.Select(s => s.SearchAsync(criteria));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).OrderByDescending(x => x.PublishedAt).ToList();
    }
}
