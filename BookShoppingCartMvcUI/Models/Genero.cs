using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUI.Models
{
    [Table("Genero")]
    public class Genero
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string GeneroName { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
