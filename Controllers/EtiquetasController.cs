using Microsoft.AspNetCore.Mvc;
using ProdutosApi.DTOs;
using ProdutosApi.Services;

namespace ProdutosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EtiquetasController : ControllerBase
{
    private readonly IEtiquetaService _service;

    public EtiquetasController(IEtiquetaService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EtiquetaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EtiquetaResponse>>> Listar([FromQuery] ProdutoFiltro filtro, CancellationToken ct)
        => Ok(await _service.ListarAsync(filtro, ct));

    [HttpGet("{id:int}", Name = "ObterEtiqueta")]
    [ProducesResponseType(typeof(EtiquetaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EtiquetaResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await _service.ObterPorIdAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EtiquetaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EtiquetaResponse>> Criar([FromBody] EtiquetaRequest request, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(request, ct);
        return CreatedAtRoute("ObterEtiqueta", new { id = criada.Id }, criada);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EtiquetaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EtiquetaResponse>> Atualizar(int id, [FromBody] EtiquetaRequest request, CancellationToken ct)
    {
        return Ok(await _service.AtualizarAsync(id, request, ct));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
