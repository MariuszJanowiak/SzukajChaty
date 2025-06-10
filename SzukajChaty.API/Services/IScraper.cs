using SzukajChaty.API.Models;

namespace SzukajChaty.API.Services;

public interface IScraper
{
    Task<List<Listing>> SearchAsync(SearchCriteria criteria);
    string SourceName { get; }
}