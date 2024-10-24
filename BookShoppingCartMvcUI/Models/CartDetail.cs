using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int ProdutoId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }   
        public Produto Produto { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
