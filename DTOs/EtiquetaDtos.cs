using System.ComponentModel.DataAnnotations;

namespace ProdutosApi.DTOs;

public class EtiquetaDtos
{
    public record EtiquetaResumo(int Id, string Nome, string? Descricao);
    
}

public record EtiquetaResponse(
    int Id,
    string Nome,
    string? Descricao
);

public class EtiquetaRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }
}