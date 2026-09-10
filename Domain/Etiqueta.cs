using System.Text.Json.Serialization;

namespace ProdutosApi.Domain;

public class Etiqueta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
   // [JsonIgnore]
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}