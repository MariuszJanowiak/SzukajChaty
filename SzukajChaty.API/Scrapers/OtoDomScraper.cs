using HtmlAgilityPack;
using SzukajChaty.API.Models;
using SzukajChaty.API.Services;
using System.Net;
using System.Web;

namespace SzukajChaty.API.Scrapers;

public class OtoDomScraper : IScraper
{
    public string SourceName => "OtoDom";

    public async Task<List<Listing>> SearchAsync(SearchCriteria criteria)
    {
        var listings = new List<Listing>();

        string? city = HttpUtility.UrlEncode(criteria.City);
        string url = $"https://www.otodom.pl/pl/oferty/sprzedaz/mieszkanie/{city}?limit=20";

        var web = new HtmlWeb();
        web.PreRequest = request =>
        {
            request.UserAgent = "Mozilla/5.0"; // Otodom może blokować boty
            return true;
        };

        HtmlDocument doc = await web.LoadFromWebAsync(url);

        var nodes = doc.DocumentNode.SelectNodes("//article");

        if (nodes == null)
            return listings;

        foreach (var node in nodes)
        {
            try
            {
                var titleNode = node.SelectSingleNode(".//p[contains(@data-cy, 'listing-item-title')]");
                var priceNode = node.SelectSingleNode(".//span[@data-cy='listing-item-price']");
                var locationNode = node.SelectSingleNode(".//p[contains(@data-cy, 'listing-item-location')]");
                var dateNode = node.SelectSingleNode(".//time");
                var imgNode = node.SelectSingleNode(".//img");

                var linkNode = node.SelectSingleNode(".//a[@href]");

                string urlPath = linkNode?.GetAttributeValue("href", string.Empty) ?? "";
                string fullUrl = urlPath.StartsWith("http") ? urlPath : $"https://www.otodom.pl{urlPath}";

                listings.Add(new Listing
                {
                    Title = WebUtility.HtmlDecode(titleNode?.InnerText?.Trim() ?? ""),
                    Price = ParsePrice(priceNode?.InnerText),
                    Location = locationNode?.InnerText?.Trim() ?? "",
                    PublishedAt = ParseDate(dateNode?.GetAttributeValue("datetime", null!)),
                    ImageUrl = imgNode?.GetAttributeValue("src", "") ?? "",
                    Url = fullUrl,
                    Source = SourceName
                });
            }
            catch
            {
                // ignoruj pojedyncze błędy parsowania
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

    private DateTime ParseDate(string? datetime)
    {
        if (DateTime.TryParse(datetime, out var dt))
            return dt;
        return DateTime.Now;
    }
}
