using PromoHunter.Domain.Entities;

namespace PromoHunter.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Produto>> GetAllAtivosAsync(CancellationToken cancellationToken = default);
    Task<List<Produto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Produto produto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default);
    Task AddNotificacaoAsync(Notificacao notificacao, CancellationToken cancellationToken = default);
}
