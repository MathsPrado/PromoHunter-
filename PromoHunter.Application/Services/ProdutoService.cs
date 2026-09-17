using PromoHunter.Domain.Entities;
using PromoHunter.Domain.Interfaces;

namespace PromoHunter.Application.Services;

public interface IProdutoService
{
    Task<Produto> AdicionarProdutoAsync(string nome, string url, decimal precoAlvo, CancellationToken ct = default);
    Task DesativarProdutoAsync(Guid id, CancellationToken ct = default);
}

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Produto> AdicionarProdutoAsync(string nome, string url, decimal precoAlvo, CancellationToken ct = default)
    {
        var produto = new Produto(nome, url, precoAlvo);
        await _repository.AddAsync(produto, ct);
        return produto;
    }

    public async Task DesativarProdutoAsync(Guid id, CancellationToken ct = default)
    {
        var produto = await _repository.GetByIdAsync(id, ct);
        if (produto != null)
        {
            produto.Desativar();
            await _repository.UpdateAsync(produto, ct);
        }
    }
}
