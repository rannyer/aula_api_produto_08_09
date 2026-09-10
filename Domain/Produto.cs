using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ProdutosApi.Domain;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    
    public ICollection<Etiqueta> Etiquetas { get; set; } = new List<Etiqueta>();
    
}
