using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models
{
    [Table("Produto")]
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? ProdutoName { get; set; }
        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }
        [Required]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }
        public Estoque Estoque { get; set; }

        [NotMapped]
        public string GeneroName { get; set; }
        [NotMapped]
        public int Quantity { get; set; }


    }
}
