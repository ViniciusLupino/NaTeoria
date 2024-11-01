namespace BookShoppingCartMvcUI.Models.DTOs
{
    public class ProdutoDisplayModel
    {
        public IEnumerable<Produto> Produtos { get; set; }
        public IEnumerable<Genero> Generos { get; set; }
        public string STerm { get; set; } = "";
        public int GeneroId { get; set; } = 0;
    }
}
