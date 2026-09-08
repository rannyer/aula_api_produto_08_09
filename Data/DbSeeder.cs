using Microsoft.EntityFrameworkCore;
using ProdutosApi.Domain;

namespace ProdutosApi.Data;

public static class DbSeeder
{
    public static async Task PopularAsync(AppDbContext context)
    {
        if (await context.Produtos.AnyAsync())
            return;

        var categorias = new[] { "Teclado", "Mouse", "Monitor", "Headset", "Webcam", "Cadeira", "Notebook" };
        var random = new Random(42);

        var produtos = Enumerable.Range(1, 47).Select(i => new Produto
        {
            Nome = $"{categorias[i % categorias.Length]} Modelo {i:D3}",
            Descricao = $"Produto de demonstração número {i}.",
            Preco = Math.Round((decimal)(random.NextDouble() * 4500 + 50), 2),
            Estoque = random.Next(0, 200),
            CriadoEm = DateTime.UtcNow.AddDays(-random.Next(0, 365))
        });

        await context.Produtos.AddRangeAsync(produtos);
        await context.SaveChangesAsync();
    }
}
