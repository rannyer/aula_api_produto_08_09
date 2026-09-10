using Microsoft.EntityFrameworkCore;
using ProdutosApi.Domain;

namespace ProdutosApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entidade =>
        { 
            entidade.HasMany(p => p.Etiquetas)
                    .WithMany(e => e.Produtos)
                    .UsingEntity(j =>
                    {
                            j.ToTable("produto_etiquetas");
                            j.Property("ProdutosId").HasColumnName("produtos_id");
                            j.Property("EtiquetasId").HasColumnName("etiquetas_id");
                    });
            
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

        modelBuilder.Entity<Etiqueta>(entidade =>
        {
                entidade.ToTable("etiquetas");
                entidade.HasKey(p => p.Id);

                entidade.Property(p => p.Nome)
                        .IsRequired()
                        .HasMaxLength(60);
                entidade.Property(p => p.Descricao)
                        .HasMaxLength(500);
                entidade.HasIndex(p => p.Nome)
                        .IsUnique()
                        .HasDatabaseName("ix_etiquetas_nome");
        });
    }
}
