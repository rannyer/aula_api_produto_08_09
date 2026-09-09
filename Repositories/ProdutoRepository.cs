using Microsoft.EntityFrameworkCore;
using ProdutosApi.Data;
using ProdutosApi.Domain;
using ProdutosApi.DTOs;

namespace ProdutosApi.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context) => _context = context;

    private static IQueryable<Produto> AplicarFiltro(IQueryable<Produto> query, ProdutoFiltro filtro)
    {
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var termo = filtro.Nome.Trim();
            query = query.Where(p => EF.Functions.Like(p.Nome, $"%{termo}%"));
        }
            
        if(filtro.PrecoMinimo.HasValue)
        {
            query = query.Where(p => p.Preco >= filtro.PrecoMinimo.Value);
        }

        if(filtro.PrecoMaximo.HasValue)
        {
            query = query.Where(p => p.Preco <= filtro.PrecoMaximo.Value);
        }

        return query;
    }
    
    private static IQueryable<Produto> AplicarOrdenacao(IQueryable<Produto> query, ProdutoFiltro filtro)
    {
        var ordenada = filtro.OrderBy?.ToLowerInvariant() switch
        {
            "nome" => filtro.Desc ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome),
            "preco" => filtro.Desc ? query.OrderByDescending(p => p.Preco) : query.OrderBy(p => p.Preco),
            "estoque" => filtro.Desc ? query.OrderByDescending(p => p.Estoque) : query.OrderBy(p => p.Estoque),
            "criadoem" => filtro.Desc ? query.OrderByDescending(p => p.CriadoEm) : query.OrderBy(p => p.CriadoEm),
            _ => filtro.Desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
        };
        return ordenada.ThenBy(p => p.Id);
    }

    public async Task<PagedResult<Produto>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default)
    {
        IQueryable<Produto> query = _context.Produtos.AsNoTracking();
        
        query = AplicarFiltro(query, filtro);
        
        var total = await query.CountAsync(ct);
        
        query = AplicarOrdenacao(query, filtro);
        
        var itens = await query
            .Skip(filtro.Skip)
            .Take(filtro.PageSize)
            .ToListAsync(ct);
        
        return new PagedResult<Produto>(itens, filtro.Page, filtro.PageSize, total);
        
        
    }
       

    public Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _context.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default) =>
        _context.Produtos
            .AsNoTracking()
            .AnyAsync(p => p.Nome.ToLower() == nome.ToLower() && (ignorarId == null || p.Id != ignorarId), ct);

    public async Task AdicionarAsync(Produto produto, CancellationToken ct = default) =>
        await _context.Produtos.AddAsync(produto, ct);

    public void Remover(Produto produto) => _context.Produtos.Remove(produto);

    public Task<int> SalvarAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
