using MassTransit;
using PromoHunter.Application.Events;
using PromoHunter.Domain.Entities;
using PromoHunter.Domain.Interfaces;
using PromoHunter.Infrastructure.Notificacoes;
using Microsoft.Extensions.Logging;

namespace PromoHunter.Worker.Consumers;

public class PromocaoEventConsumer : IConsumer<PromocaoEncontradaEvent>
{
    private readonly ITelegramBotService _botService;
    private readonly IProdutoRepository _repo;
    private readonly ILogger<PromocaoEventConsumer> _logger;

    public PromocaoEventConsumer(ITelegramBotService botService, IProdutoRepository repo, ILogger<PromocaoEventConsumer> logger)
    {
        _botService = botService;
        _repo = repo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PromocaoEncontradaEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processando envio para: {NomeProduto}", msg.NomeProduto);

        string texto = $"🔥 *PROMOÇÃO* 🔥\n{msg.NomeProduto}\nAlvo: R${msg.PrecoAlvo:N2} | Atual: R${msg.PrecoAtual:N2}\nLink: {msg.Url}";

        bool sucesso = true;
        string? erro = null;

        try
        {
            await _botService.EnviarMensagemAsync(texto, context.CancellationToken);
        }
        catch (Exception ex)
        {
            sucesso = false;
            erro = ex.Message;
            _logger.LogError(ex, "Erro no bot Telegram");
        }

        var notificacao = new Notificacao(msg.ProdutoId, msg.PrecoAtual, sucesso, erro);
        await _repo.AddNotificacaoAsync(notificacao, context.CancellationToken);

        // Rate limiting of Telegram API
        await Task.Delay(TimeSpan.FromSeconds(3), context.CancellationToken);
    }
}
