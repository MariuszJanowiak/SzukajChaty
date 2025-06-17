using HtmlAgilityPack;
using SzukajChaty.Shared.Models;
using SzukajChaty.API.Services;
using System.Net;
using System.Web;

namespace Szukaj.API.Scrapers;

public class OlxScraper : IScraper
{
    public string SourceName => "OLX";

    public async Task<List<Listing>> SearchAsync(SearchCriteria criteria)
    {
        var listings = new List<Listing>();

        string? city = HttpUtility.UrlEncode(criteria.City);
        string url = $"https://www.olx.pl/nieruchomosci/mieszkania/sprzedaz/{city}/?search%5Border%5D=created_at:desc";

        var web = new HtmlWeb();
        web.PreRequest = request =>
        {
            request.UserAgent = "Mozilla/5.0";
            return true;
        };

        HtmlDocument doc = await web.LoadFromWebAsync(url);

        var nodes = doc.DocumentNode.SelectNodes("//div[@data-cy='l-card']");

        if (nodes == null)
            return listings;

        foreach (var node in nodes)
        {
            try
            {
                var titleNode = node.SelectSingleNode(".//h6");
                var priceNode = node.SelectSingleNode(".//p[contains(@data-testid,'ad-price')]");
                var locationNode = node.SelectSingleNode(".//p[contains(@data-testid,'location-date')]");
                var linkNode = node.SelectSingleNode(".//a[@href]");
                var imgNode = node.SelectSingleNode(".//img");

                string fullUrl = "https://www.olx.pl" + (linkNode?.GetAttributeValue("href", "") ?? "");

                listings.Add(new Listing
                {
                    Title = WebUtility.HtmlDecode(titleNode?.InnerText?.Trim() ?? ""),
                    Price = ParsePrice(priceNode?.InnerText),
                    Location = locationNode?.InnerText?.Split('-')?.FirstOrDefault()?.Trim() ?? "",
                    PublishedAt = DateTime.Now,
                    ImageUrl = imgNode?.GetAttributeValue("src", "") ?? "",
                    Url = fullUrl,
                    Source = SourceName
                });
            }
            catch
            {
            }
        }

        return listings;
    }

    private decimal ParsePrice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;
        var cleaned = new string(input.Where(char.IsDigit).ToArray());
        return decimal.TryParse(cleaned, out var result) ? result : 0;
    }
}
