namespace PromoHunter.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Url { get; private set; }
    public decimal PrecoAlvo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    
    private readonly List<HistoricoPreco> _historico = new();
    public IReadOnlyCollection<HistoricoPreco> Historico => _historico.AsReadOnly();

    private readonly List<Notificacao> _notificacoes = new();
    public IReadOnlyCollection<Notificacao> Notificacoes => _notificacoes.AsReadOnly();

    protected Produto() { } // EF Core req

    public Produto(string nome, string url, decimal precoAlvo)
    {
        Id = Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome inválido") : nome;
        Url = string.IsNullOrWhiteSpace(url) ? throw new ArgumentException("URL inválida") : url;
        PrecoAlvo = precoAlvo > 0 ? precoAlvo : throw new ArgumentException("Preço deve ser maior que zero");
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
    public void AtualizarPrecoAlvo(decimal precoAlvo) => PrecoAlvo = precoAlvo;

    public bool AnalisarNovoPreco(decimal precoAtual)
    {
        var historico = new HistoricoPreco(Id, precoAtual);
        _historico.Add(historico);

        return precoAtual <= PrecoAlvo;
    }
}
