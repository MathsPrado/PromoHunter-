namespace PromoHunter.Application.Events;

public record PromocaoEncontradaEvent
{
    public Guid ProdutoId { get; init; }
    public string NomeProduto { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public decimal PrecoAlvo { get; init; }
    public decimal PrecoAtual { get; init; }
}
