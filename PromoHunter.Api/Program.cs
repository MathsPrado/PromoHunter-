using MassTransit;
using Microsoft.EntityFrameworkCore;
using PromoHunter.Application.Services;
using PromoHunter.Domain.Interfaces;
using PromoHunter.Infrastructure.Data;
using PromoHunter.Infrastructure.Data.Repositories;
using PromoHunter.Infrastructure.Scraping;
using PromoHunter.Api.BackgroundServices;
using PromoHunter.Domain.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Database
builder.Services.AddDbContext<PromoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=promohunter.db"));

// Scoped Dependencies
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddHttpClient<IProvedorPreco, HtmlAgilityScraper>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// MassTransit config
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// Background Services
builder.Services.AddHostedService<ScraperBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => 
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints (Minimal API)
app.MapGet("/api/produtos", async (IProdutoRepository repo) =>
{
    return Results.Ok(await repo.GetAllAsync());
});

app.MapPost("/api/produtos", async (ProdutoModel model, IProdutoService service) =>
{
    var p = await service.AdicionarProdutoAsync(model.Nome, model.Url, model.PrecoAlvo);
    return Results.Ok(p);
});

app.MapGet("/api/produtos/{id:guid}", async (Guid id, IProdutoRepository repo) =>
{
    var p = await repo.GetByIdAsync(id);
    return p != null ? Results.Ok(p) : Results.NotFound();
});

app.MapPost("/api/produtos/{id:guid}/toggle", async (Guid id, IProdutoRepository repo) =>
{
    var p = await repo.GetByIdAsync(id);
    if (p == null) return Results.NotFound();
    
    if (p.Ativo) p.Desativar(); 
    else p.Ativar();
    
    await repo.UpdateAsync(p);
    return Results.Ok(p);
});

app.MapGet("/api/logs", async (PromoDbContext ctx) =>
{
    // Apenas para visualizacao de logs centralizados
    return Results.Ok(await ctx.Notificacoes.OrderByDescending(n => n.DataEnvio).Take(50).ToListAsync());
});

app.Run();

public record ProdutoModel(string Nome, string Url, decimal PrecoAlvo);
