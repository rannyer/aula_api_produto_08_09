using ProdutosApi.Domain;
using ProdutosApi.DTOs;

namespace ProdutosApi.Repositories;

public interface IProdutoRepository
{
    Task<PagedResult<Produto>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default);
    Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default);
    Task AdicionarAsync(Produto produto, CancellationToken ct = default);
    void Remover(Produto produto);
    Task<int> SalvarAsync(CancellationToken ct = default);
    Task<Etiqueta?> ObterEtiquetaPorIdAsync(int etiquetaId, CancellationToken ct = default);
    Task<bool> EtiquetaJaAssociadaAsync(int produtoId, int etiquetaId, CancellationToken ct = default);
}
