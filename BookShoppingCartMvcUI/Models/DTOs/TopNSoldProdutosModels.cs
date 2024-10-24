namespace BookShoppingCartMvcUI.Models.DTOs;

public record TopNSoldProdutoModel(string ProdutoName, string AuthorName, int TotalUnitSold);
public record TopNSoldProdutosVm(DateTime StartDate, DateTime EndDate, IEnumerable<TopNSoldProdutoModel> TopNSoldBooks);