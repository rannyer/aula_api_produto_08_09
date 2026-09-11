using ProdutosApi.DTOs;

namespace ProdutosApi.Services;

public interface IEtiquetaService
{
    Task<PagedResult<EtiquetaResponse>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default);
    Task<EtiquetaResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<EtiquetaResponse> CriarAsync(EtiquetaRequest request, CancellationToken ct = default);
    Task<EtiquetaResponse> AtualizarAsync(int id, EtiquetaRequest request, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
}
