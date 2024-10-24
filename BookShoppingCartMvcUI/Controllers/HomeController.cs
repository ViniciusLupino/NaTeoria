using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShoppingCartMvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm="",int GeneroId=0)
        {
            IEnumerable<Produto> Produtos = await _homeRepository.GetProdutos(sterm, GeneroId);
            IEnumerable<Genero> Generos = await _homeRepository.Generos();
            ProdutoDisplayModel bookModel = new ProdutoDisplayModel
            {
              Produtos=Produtos,
              Generos=Generos,
              STerm=sterm,
              GeneroId=GeneroId
            };
            return View(bookModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}