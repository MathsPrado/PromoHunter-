using Microsoft.EntityFrameworkCore;
using PromoHunter.Domain.Entities;

namespace PromoHunter.Infrastructure.Data;

public class PromoDbContext : DbContext
{
    public PromoDbContext(DbContextOptions<PromoDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<HistoricoPreco> HistoricosPreco => Set<HistoricoPreco>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired();
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.PrecoAlvo).HasColumnType("decimal(18,2)");
            
            var navigation = entity.Metadata.FindNavigation(nameof(Produto.Historico));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

            var notificacaoNav = entity.Metadata.FindNavigation(nameof(Produto.Notificacoes));
            notificacaoNav?.SetPropertyAccessMode(PropertyAccessMode.Field);
            
            entity.HasMany(e => e.Historico).WithOne().HasForeignKey(e => e.ProdutoId);
            entity.HasMany(e => e.Notificacoes).WithOne().HasForeignKey(e => e.ProdutoId);
        });

        modelBuilder.Entity<HistoricoPreco>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoMomento).HasColumnType("decimal(18,2)");
        });
    }
}
