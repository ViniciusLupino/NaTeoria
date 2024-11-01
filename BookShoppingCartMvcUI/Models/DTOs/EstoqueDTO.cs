using System.ComponentModel.DataAnnotations;

namespace EcoImpulse.Models.DTOs
{
    public class EstoqueDTO
    {
        public int ProdutoId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }
    }
}
