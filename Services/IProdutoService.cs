using ProdutosApi.DTOs;

namespace ProdutosApi.Services;

public interface IProdutoService
{
    Task<PagedResult<ProdutoResponse>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default);
    Task<ProdutoResponse?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<ProdutoResponse> CriarAsync(ProdutoRequest request, CancellationToken ct = default);
    Task<ProdutoResponse?> AtualizarAsync(int id, ProdutoRequest request, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
