namespace SzukajChaty.Shared.Models;

public class Listing
{
    public string? Title { get; set; }
    public string? Url { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime PublishedAt { get; set;}
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? Source { get; set; }
}
