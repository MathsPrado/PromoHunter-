using Telegram.Bot;

namespace PromoHunter.Infrastructure.Notificacoes;

public interface ITelegramBotService
{
    Task EnviarMensagemAsync(string mensagem, CancellationToken ct = default);
}

public class TelegramBotService : ITelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly long _chatId;

    public TelegramBotService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        // In real world, get this from IConfiguration
        _chatId = 123456789; // Mock
    }

    public async Task EnviarMensagemAsync(string mensagem, CancellationToken ct = default)
    {
        try
        {
            await _botClient.SendMessage(
                chatId: _chatId,
                text: mensagem,
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro enviado telegram: {ex.Message}");
        }
    }
}
