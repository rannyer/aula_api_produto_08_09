using Microsoft.EntityFrameworkCore;
using ProdutosApi.Domain;

namespace ProdutosApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entidade =>
        {
            entidade.ToTable("produtos");
            entidade.HasKey(p => p.Id);

            entidade.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(120);

            entidade.Property(p => p.Descricao)
                    .HasMaxLength(500);

            entidade.Property(p => p.Preco)
                    .HasPrecision(10, 2)
                    .IsRequired();

            entidade.Property(p => p.CriadoEm)
                    .HasColumnType("timestamptz")
                    .IsRequired();

            entidade.HasIndex(p => p.Nome)
                    .IsUnique()
                    .HasDatabaseName("ix_produtos_nome");

            entidade.HasIndex(p => p.Preco);
        });
    }
}
