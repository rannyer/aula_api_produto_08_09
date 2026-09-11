using ProdutosApi.Domain;
using ProdutosApi.DTOs;

namespace ProdutosApi.Repositories;

public interface IEtiquetaRepository
{
    Task<PagedResult<Etiqueta>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default);
    Task<Etiqueta?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default);
    Task AdicionarAsync(Etiqueta etiqueta, CancellationToken ct = default);
    void Remover(Etiqueta etiqueta);
    Task<int> SalvarAsync(CancellationToken ct = default);
}
