using ProdutosApi.Domain;
using ProdutosApi.DTOs;
using ProdutosApi.Exceptions;
using ProdutosApi.Repositories;

namespace ProdutosApi.Services;

public class EtiquetaService : IEtiquetaService
{
    private readonly IEtiquetaRepository _repository;

    public EtiquetaService(IEtiquetaRepository repository) => _repository = repository;

    public async Task<PagedResult<EtiquetaResponse>> ListarAsync(ProdutoFiltro filtro, CancellationToken ct = default)
    {
        var pagina = await _repository.ListarAsync(filtro, ct);
        return new PagedResult<EtiquetaResponse>(
            pagina.Items.Select(Mapear).ToList(),
            pagina.Page,
            pagina.PageSize,
            pagina.TotalItems
        );
    }

    public async Task<EtiquetaResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var etiqueta = await _repository.ObterPorIdAsync(id, ct) 
            ?? throw new RecursoNaoEncontradoException("Etiqueta", id);
        
        return Mapear(etiqueta);
    }

    public async Task<EtiquetaResponse> CriarAsync(EtiquetaRequest request, CancellationToken ct = default)
    {
        var nome = request.Nome.Trim();
        
        if(await _repository.ExisteNomeAsync(nome, null, ct))
            throw new ConflitoException($"Já existe uma etiqueta chamada '{nome}'.");
        
        var etiqueta = new Etiqueta
        {
            Nome = nome,
            Descricao = request.Descricao?.Trim()
        };
        
        await _repository.AdicionarAsync(etiqueta, ct);
        await _repository.SalvarAsync(ct);

        return Mapear(etiqueta);
    }

    public async Task<EtiquetaResponse> AtualizarAsync(int id, EtiquetaRequest request, CancellationToken ct = default)
    {
        var etiqueta = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException("Etiqueta", id);

        var nome = request.Nome.Trim();

        if(await _repository.ExisteNomeAsync(nome, id, ct))
            throw new ConflitoException($"Já existe uma etiqueta chamada '{nome}'.");
        
        etiqueta.Nome = nome;
        etiqueta.Descricao = request.Descricao?.Trim();

        await _repository.SalvarAsync(ct);

        return Mapear(etiqueta);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var etiqueta = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException("Etiqueta", id);

        _repository.Remover(etiqueta);
        await _repository.SalvarAsync(ct);

        return true;
    }

    private static EtiquetaResponse Mapear(Etiqueta e) =>
        new(e.Id, e.Nome, e.Descricao);
}
