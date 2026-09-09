using Microsoft.AspNetCore.Mvc;
using ProdutosApi.DTOs;
using ProdutosApi.Services;

namespace ProdutosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProdutoResponse>>> Listar([FromQuery] ProdutoFiltro filtro, CancellationToken ct)
        => Ok(await _service.ListarAsync(filtro, ct));
// Http://localhost:8080/api/produtos?page=2&pageSize=5&precoMin=20


    [HttpGet("{id:int}", Name = "ObterProduto")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await _service.ObterPorIdAsync(id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponse>> Criar([FromBody] ProdutoRequest request, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(request, ct);
        return CreatedAtRoute("ObterProduto", new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(int id, [FromBody] ProdutoRequest request, CancellationToken ct)
    {
        return Ok( await _service.AtualizarAsync(id, request, ct));
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
