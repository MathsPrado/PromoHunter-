using MassTransit;
using PromoHunter.Application.Events;
using PromoHunter.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace PromoHunter.Api.BackgroundServices;

public class ScraperBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScraperBackgroundService> _logger;

    public ScraperBackgroundService(
        IServiceProvider serviceProvider, 
        ILogger<ScraperBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando varredura de preços: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
                var provedor = scope.ServiceProvider.GetRequiredService<IProvedorPreco>();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var produtosAtivos = await repo.GetAllAtivosAsync(stoppingToken);

                foreach (var produto in produtosAtivos)
                {
                    var price = await provedor.ObterPrecoAtualAsync(produto.Url, stoppingToken);
                    if (price.HasValue)
                    {
                        var hasPromocao = produto.AnalisarNovoPreco(price.Value);
                        await repo.UpdateAsync(produto, stoppingToken); 

                        if (hasPromocao)
                        {
                            var evt = new PromocaoEncontradaEvent
                            {
                                ProdutoId = produto.Id,
                                NomeProduto = produto.Nome,
                                Url = produto.Url,
                                PrecoAlvo = produto.PrecoAlvo,
                                PrecoAtual = price.Value
                            };
                            await publisher.Publish(evt, stoppingToken);
                            _logger.LogInformation($"Promoção encontrada para {produto.Nome}! Evento publicado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no ciclo do background service.");
            }

            // Aguarda 1 minuto
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
