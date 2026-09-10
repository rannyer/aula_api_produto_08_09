using System.ComponentModel.DataAnnotations;

namespace ProdutosApi.DTOs;

public record ProdutoResponse(
    int Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    int Estoque,
    DateTime CriadoEm,
    IReadOnlyList<EtiquetaDtos.EtiquetaResumo> Etiquetas
    );

public class ProdutoRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    [Range(0.01, 1_000_000, ErrorMessage = "O preço deve estar entre 0,01 e 1.000.000,00.")]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }
}
