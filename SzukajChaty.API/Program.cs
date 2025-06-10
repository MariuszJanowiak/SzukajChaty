using Szukaj.API.Scrapers;
using SzukajChaty.API.Services;
using SzukajChaty.API.Scrapers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ScraperService>();
builder.Services.AddScoped<IScraper, OtoDomScraper>();
builder.Services.AddScoped<IScraper, OlxScraper>();
builder.Services.AddScoped<IScraper, NieruchomosciOnlineScraper>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
