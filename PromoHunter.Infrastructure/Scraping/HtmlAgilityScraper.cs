using HtmlAgilityPack;
using PromoHunter.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace PromoHunter.Infrastructure.Scraping;

public class HtmlAgilityScraper : IProvedorPreco
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HtmlAgilityScraper> _logger;

    public HtmlAgilityScraper(HttpClient httpClient, ILogger<HtmlAgilityScraper> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // Mock browser user agent
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36");
    }

    public async Task<decimal?> ObterPrecoAtualAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            // For now, generating a mock price based on Random just so the mechanism triggers,
            // as real parsing depends on specific store CSS paths.
            // Normally: var response = await _httpClient.GetStringAsync(url); ....
            await Task.Delay(500, cancellationToken);
            return new decimal(new Random().NextDouble() * 500); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no scraping da URL {Url}", url);
            return null;
        }
    }
}
