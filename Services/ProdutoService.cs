using ProdutosApi.Domain;
using ProdutosApi.DTOs;
using ProdutosApi.Exceptions;
using ProdutosApi.Repositories;

namespace ProdutosApi.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository) => _repository = repository;

    public async Task<PagedResult<ProdutoResponse>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default)
    {
        var pagina = await _repository.ListarAsync(filtro, ct);
        return new PagedResult<ProdutoResponse>(
            pagina.Items.Select(Mapear).ToList(),
            pagina.Page,
            pagina.PageSize,
            pagina.TotalItems
        );
    }

    public async Task<ProdutoResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var produto = await _repository.ObterPorIdAsync(id, ct) 
            ?? throw new RecursoNaoEncontradoException("Produto", id);
        
        return Mapear(produto);
        
    }

    public async Task<ProdutoResponse> CriarAsync(ProdutoRequest request, CancellationToken ct = default)
    {
        var nome = request.Nome.Trim();
        
        if(await _repository.ExisteNomeAsync(nome, null, ct))
            throw new ConflitoException($"Ja existe um produto chamado '{nome}'.");
        
        var produto = new Produto
        {
            Nome = request.Nome.Trim(),
            Descricao = request.Descricao?.Trim(),
            Preco = request.Preco,
            Estoque = request.Estoque,
            CriadoEm = DateTime.UtcNow
        };
        

        await _repository.AdicionarAsync(produto, ct);
        await _repository.SalvarAsync(ct);

        return Mapear(produto);
    }

    public async Task<ProdutoResponse> AtualizarAsync(int id, ProdutoRequest request, CancellationToken ct = default)
    {
        var produto = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException("Produto", id);

        var nome = request.Nome.Trim();

        if(await _repository.ExisteNomeAsync(nome, null, ct))
            throw new ConflitoException($"Ja existe um produto chamado '{nome}'.");
        
        if(produto.Estoque > 0 && request.Estoque == 0 && request.Preco > produto.Preco * 2)
           throw new RegraNegocioException("Não é permitido reduzir o estoque para zero e aumentar o preço em mais de 100%.");
        
        
        produto.Nome = nome;
        produto.Descricao = request.Descricao?.Trim();
        produto.Preco = request.Preco;
        produto.Estoque = request.Estoque;

        await _repository.SalvarAsync(ct);

        return Mapear(produto);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var produto = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException("Produto", id);

        _repository.Remover(produto);
        await _repository.SalvarAsync(ct);

        return true;
    }

    public async Task<ProdutoResponse> AdicionarEtiquetaAsync(int produtoId, int etiquetaId, CancellationToken ct = default)
    {
        var produto = await _repository.ObterPorIdAsync(produtoId, ct)
            ?? throw new RecursoNaoEncontradoException("Produto", produtoId);

        var etiqueta = await _repository.ObterEtiquetaPorIdAsync(etiquetaId, ct)
            ?? throw new RecursoNaoEncontradoException("Etiqueta", etiquetaId);

        if (await _repository.EtiquetaJaAssociadaAsync(produtoId, etiquetaId, ct))
            throw new ConflitoException($"A etiqueta '{etiqueta.Nome}' já está associada ao produto '{produto.Nome}'.");

        produto.Etiquetas.Add(etiqueta);
        await _repository.SalvarAsync(ct);

        return Mapear(produto);
    }

    private static ProdutoResponse Mapear(Produto p) =>
        new(p.Id, p.Nome, p.Descricao, p.Preco, p.Estoque, p.CriadoEm, 
            p.Etiquetas.Select(e => new EtiquetaDtos.EtiquetaResumo(e.Id, e.Nome, e.Descricao)).ToList());
}
