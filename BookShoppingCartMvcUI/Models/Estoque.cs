using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models
{
    [Table("Estoque")]
    public class Estoque
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
    }
}
