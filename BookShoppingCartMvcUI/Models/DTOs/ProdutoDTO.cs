using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingCartMvcUI.Models.DTOs;
public class ProdutoDTO
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
    public IFormFile? ImageFile { get; set; }
    public IEnumerable<SelectListItem>? GenreList { get; set; }
}
