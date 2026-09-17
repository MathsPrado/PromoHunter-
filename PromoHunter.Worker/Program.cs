using MassTransit;
using Microsoft.EntityFrameworkCore;
using PromoHunter.Domain.Interfaces;
using PromoHunter.Infrastructure.Data;
using PromoHunter.Infrastructure.Data.Repositories;
using PromoHunter.Infrastructure.Notificacoes;
using PromoHunter.Worker.Consumers;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

// Bot registration
builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var botToken = builder.Configuration["TelegramBotToken"] ?? "YOUR_TELEGRAM_TOKEN_HERE";
        TelegramBotClientOptions options = new(botToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<ITelegramBotService, TelegramBotService>();

builder.Services.AddDbContext<PromoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=../PromoHunter.Api/promohunter.db"));

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PromocaoEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("promo-events", e =>
        {
            // Politica de Retentativa
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<PromocaoEventConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
