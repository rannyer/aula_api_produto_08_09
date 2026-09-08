using Microsoft.EntityFrameworkCore;
using ProdutosApi.Data;
using ProdutosApi.Domain;
using ProdutosApi.DTOs;

namespace ProdutosApi.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<Produto>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default)
    {
        IQueryable<Produto> query = _context.Produtos.AsNoTracking();
        
        var total = await query.CountAsync(ct);
        
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
