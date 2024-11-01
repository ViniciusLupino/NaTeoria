namespace EcoImpulse.Models.DTOs
{
    public class EstoqueDisplayModel
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantity { get; set; }
        public string? ProdutoName { get; set; }
    }
}
