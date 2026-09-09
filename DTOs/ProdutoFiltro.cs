namespace ProdutosApi.DTOs;

public class ProdutoFiltro
{
    public const int TamanhoMaximoPagina = 10;

    private int _page = 1;
    private int _pagesize = 10;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pagesize;
        set => _pagesize = value switch
        {
            < 1 => 1,
            > TamanhoMaximoPagina => TamanhoMaximoPagina,
            _ => value
        };
    }
    
    public int Skip => (Page - 1) * PageSize;
    
    public string? Nome { get; set; }
    public decimal? PrecoMinimo { get; set; }
    public decimal? PrecoMaximo { get; set; }
    public string OrderBy { get; set; }
    public bool Desc { get; set; }
    

}