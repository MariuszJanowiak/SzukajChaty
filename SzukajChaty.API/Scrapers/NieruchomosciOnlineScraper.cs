using HtmlAgilityPack;
using SzukajChaty.API.Models;
using SzukajChaty.API.Services;
using System.Net;
using System.Web;

namespace SzukajChaty.API.Scrapers;

public class NieruchomosciOnlineScraper : IScraper
{
    public string SourceName => "Nieruchomosci-Online";

    public async Task<List<Listing>> SearchAsync(SearchCriteria criteria)
    {
        var listings = new List<Listing>();

        string? city = HttpUtility.UrlEncode(criteria.City?.ToLower());
        string url = $"https://www.nieruchomosci-online.pl/mieszkania,{city},sprzedaz/?nr=20";

        var web = new HtmlWeb();
        web.PreRequest = request =>
        {
            request.UserAgent = "Mozilla/5.0";
            return true;
        };

        HtmlDocument doc = await web.LoadFromWebAsync(url);

        var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'offer-item')]");

        if (nodes == null)
            return listings;

        foreach (var node in nodes)
        {
            try
            {
                var titleNode = node.SelectSingleNode(".//span[contains(@class, 'offer-title')]");
                var priceNode = node.SelectSingleNode(".//li[contains(@class, 'offer-price')]");
                var locationNode = node.SelectSingleNode(".//li[contains(@class, 'offer-location')]");
                var dateNode = node.SelectSingleNode(".//li[contains(@class, 'offer-date')]");
                var linkNode = node.SelectSingleNode(".//a[@href]");
                var imgNode = node.SelectSingleNode(".//img");

                string fullUrl = linkNode?.GetAttributeValue("href", "") ?? "";

                listings.Add(new Listing
                {
                    Title = WebUtility.HtmlDecode(titleNode?.InnerText?.Trim() ?? ""),
                    Price = ParsePrice(priceNode?.InnerText),
                    Location = locationNode?.InnerText?.Trim() ?? "",
                    PublishedAt = ParseDate(dateNode?.InnerText),
                    ImageUrl = imgNode?.GetAttributeValue("data-src", "") ?? "",
                    Url = fullUrl,
                    Source = SourceName
                });
            }
            catch
            {
                // ignoruj błędne wpisy
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

    private DateTime ParseDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return DateTime.Now;

        // Przyjmujemy że wpis jest np. "Dzisiaj, 08:12" lub "Wczoraj, 14:21"
        input = input.ToLower();
        if (input.Contains("dzisiaj")) return DateTime.Today;
        if (input.Contains("wczoraj")) return DateTime.Today.AddDays(-1);

        return DateTime.Now;
    }
}
