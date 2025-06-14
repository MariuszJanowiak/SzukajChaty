namespace SzukajChaty.API.Models;

public class SearchCriteria
{
    public string? City { get; set; }
    public string? Type { get; set; } // "flat", "house" itp.
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public List<string> Portals { get; set; } = new();
}