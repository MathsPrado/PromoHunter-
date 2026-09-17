using Microsoft.EntityFrameworkCore;
using PromoHunter.Domain.Entities;
using PromoHunter.Domain.Interfaces;

namespace PromoHunter.Infrastructure.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly PromoDbContext _context;

    public ProdutoRepository(PromoDbContext context)
    {
        _context = context;
    }

    public Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Produtos
            .Include(p => p.Historico)
            .Include(p => p.Notificacoes)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<List<Produto>> GetAllAtivosAsync(CancellationToken cancellationToken = default)
    {
        return _context.Produtos
            .Where(p => p.Ativo)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Produto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Produtos.OrderByDescending(p => p.DataCadastro).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddNotificacaoAsync(Notificacao notificacao, CancellationToken cancellationToken = default)
    {
        await _context.Notificacoes.AddAsync(notificacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
