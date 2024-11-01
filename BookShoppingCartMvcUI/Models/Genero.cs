using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoImpulse.Models
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
