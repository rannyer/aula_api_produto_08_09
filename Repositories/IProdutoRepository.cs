using ProdutosApi.Domain;

namespace ProdutosApi.Repositories;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync(CancellationToken ct = default);
    Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default);
    Task AdicionarAsync(Produto produto, CancellationToken ct = default);
    void Remover(Produto produto);
    Task<int> SalvarAsync(CancellationToken ct = default);
}
