namespace PromoHunter.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; private set; }
    public Guid ProdutoId { get; private set; }
    public DateTime DataEnvio { get; private set; }
    public decimal PrecoMomento { get; private set; }
    public bool Sucesso { get; private set; }
    public string? ErroMensagem { get; private set; }

    protected Notificacao() { }

    public Notificacao(Guid produtoId, decimal precoMomento, bool sucesso, string? erroMensagem = null)
    {
        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        PrecoMomento = precoMomento;
        Sucesso = sucesso;
        ErroMensagem = erroMensagem;
        DataEnvio = DateTime.UtcNow;
    }
}
