namespace PromoHunter.Domain.Entities;

public class HistoricoPreco
{
    public Guid Id { get; private set; }
    public Guid ProdutoId { get; private set; }
    public decimal Preco { get; private set; }
    public DateTime DataVerificacao { get; private set; }

    protected HistoricoPreco() { }

    public HistoricoPreco(Guid produtoId, decimal preco)
    {
        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        Preco = preco;
        DataVerificacao = DateTime.UtcNow;
    }
}
