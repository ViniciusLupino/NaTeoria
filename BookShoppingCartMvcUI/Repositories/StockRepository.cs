using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories
{
    public class StockRepository : IEstoqueRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Estoque?> GetEstoqueByProdutoId(int produtoId) => await _context.Stocks.FirstOrDefaultAsync(s => s.ProdutoId == produtoId);

        public async Task ManageStock(EstoqueDTO stockToManage)
        {
            // if there is no estoque for given produto id, then add new record
            // if there is already estoque for given produto id, update estoque's quantity
            var estoqueExiste = await GetEstoqueByProdutoId(stockToManage.ProdutoId);
            if (estoqueExiste is null)
            {
                var estoque = new Estoque { ProdutoId = stockToManage.ProdutoId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(estoque);
            }
            else
            {
                estoqueExiste.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EstoqueDisplayModel>> GetEstoques(string sTerm = "")
        {
            var stocks = await (from produto in _context.Produtos
                                join estoque in _context.Stocks
                                on produto.Id equals estoque.ProdutoId
                                into book_stock
                                from bookStock in book_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || produto.ProdutoName.ToLower().Contains(sTerm.ToLower())
                                select new EstoqueDisplayModel
                                {
                                    ProdutoId = produto.Id,
                                    ProdutoName = produto.ProdutoName,
                                    Quantity = bookStock == null ? 0 : bookStock.Quantity
                                }
                                ).ToListAsync();
            return stocks;
        }

    }

    public interface IEstoqueRepository
    {
        Task<IEnumerable<EstoqueDisplayModel>> GetEstoques(string sTerm = "");
        Task<Estoque?> GetEstoqueByProdutoId(int produtoId);
        Task ManageStock(EstoqueDTO stockToManage);
    }
}
