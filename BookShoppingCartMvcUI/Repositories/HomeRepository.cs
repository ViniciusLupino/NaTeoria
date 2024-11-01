

using Microsoft.EntityFrameworkCore;

namespace EcoImpulse.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genero>> Generos()
        {
            return await _db.Generos.ToListAsync();
        }
        public async Task<IEnumerable<Produto>> GetProdutos(string sTerm = "", int GeneroId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Produto> Produtos = await (from produto in _db.Produtos
                                                   join genero in _db.Generos
                                                   on produto.GeneroId equals genero.Id
                                                   join estoque in _db.Stocks
                                                   on produto.Id equals estoque.ProdutoId
                                                   into book_stocks
                                                   from bookWithStock in book_stocks.DefaultIfEmpty()
                                                   where string.IsNullOrWhiteSpace(sTerm) || (produto != null && produto.ProdutoName.ToLower().StartsWith(sTerm))
                                                   select new Produto
                                                   {
                                                       Id = produto.Id,
                                                       Image = produto.Image,
                                                       ProdutoName = produto.ProdutoName,
                                                       GeneroId = produto.GeneroId,
                                                       Price = produto.Price,
                                                       GeneroName = genero.GeneroName,
                                                       Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity
                                                   }
                         ).ToListAsync();
            if (GeneroId > 0)
            {

                Produtos = Produtos.Where(a => a.GeneroId == GeneroId).ToList();
            }
            return Produtos;

        }
    }
}
