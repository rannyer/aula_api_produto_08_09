using Microsoft.EntityFrameworkCore;
using ProdutosApi.Data;
using ProdutosApi.Domain;
using ProdutosApi.DTOs;

namespace ProdutosApi.Repositories;

public class EtiquetaRepository : IEtiquetaRepository
{
    private readonly AppDbContext _context;

    public EtiquetaRepository(AppDbContext context) => _context = context;

    private static IQueryable<Etiqueta> AplicarFiltro(IQueryable<Etiqueta> query, ProdutoFiltro filtro)
    {
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var termo = filtro.Nome.Trim();
            query = query.Where(e => EF.Functions.Like(e.Nome, $"%{termo}%"));
        }
        return query;
    }
    
    private static IQueryable<Etiqueta> AplicarOrdenacao(IQueryable<Etiqueta> query, ProdutoFiltro filtro)
    {
        var ordenada = filtro.OrderBy?.ToLowerInvariant() switch
        {
            "nome" => filtro.Desc ? query.OrderByDescending(e => e.Nome) : query.OrderBy(e => e.Nome),
            _ => filtro.Desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id)
        };
      
        return ordenada.ThenBy(e => e.Id);
    }

    public async Task<PagedResult<Etiqueta>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default)
    {
        IQueryable<Etiqueta> query = _context.Etiquetas.AsNoTracking();
        
        query = AplicarFiltro(query, filtro);
        
        var total = await query.CountAsync(ct);
        
        query = AplicarOrdenacao(query, filtro);
        
        var itens = await query
            .Skip(filtro.Skip)
            .Take(filtro.PageSize)
            .ToListAsync(ct);
        
        return new PagedResult<Etiqueta>(itens, filtro.Page, filtro.PageSize, total);
    }
       

    public Task<Etiqueta?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _context.Etiquetas.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default) =>
        _context.Etiquetas
            .AsNoTracking()
            .AnyAsync(e => e.Nome.ToLower() == nome.ToLower() && (ignorarId == null || e.Id != ignorarId), ct);

    public async Task AdicionarAsync(Etiqueta etiqueta, CancellationToken ct = default) =>
        await _context.Etiquetas.AddAsync(etiqueta, ct);

    public void Remover(Etiqueta etiqueta) => _context.Etiquetas.Remove(etiqueta);

    public Task<int> SalvarAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
