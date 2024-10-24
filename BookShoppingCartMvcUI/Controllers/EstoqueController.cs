using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class EstoqueController : Controller
    {
        private readonly IEstoqueRepository _estoqueRepo;

        public EstoqueController(IEstoqueRepository estoqueRepo)
        {
            _estoqueRepo = estoqueRepo;
        }

        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _estoqueRepo.GetEstoques(sTerm);
            return View(stocks);
        }

        public async Task<IActionResult> ManageStock(int produtoId)
        {
            var estoqueExiste = await _estoqueRepo.GetEstoqueByProdutoId(produtoId);
            var estoque = new EstoqueDTO
            {
                ProdutoId = produtoId,
                Quantity = estoqueExiste != null
            ? estoqueExiste.Quantity : 0
            };
            return View(estoque);
        }

        [HttpPost]
        public async Task<IActionResult> ManangeStock(EstoqueDTO estoque)
        {
            if (!ModelState.IsValid)
                return View(estoque);
            try
            {
                await _estoqueRepo.ManageStock(estoque);
                TempData["successMessage"] = "Estoque is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
