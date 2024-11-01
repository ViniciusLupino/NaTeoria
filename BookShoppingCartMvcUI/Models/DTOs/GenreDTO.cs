using System.ComponentModel.DataAnnotations;

namespace EcoImpulse.Models.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string GeneroName { get; set; }
    }
}
