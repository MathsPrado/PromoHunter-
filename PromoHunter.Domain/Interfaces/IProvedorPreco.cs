namespace PromoHunter.Domain.Interfaces;

public interface IProvedorPreco
{
    Task<decimal?> ObterPrecoAtualAsync(string url, CancellationToken cancellationToken = default);
}
