using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int produtoId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(produtoId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int produtoId)
        {
            var cartCount = await _cartRepo.RemoveItem(produtoId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            bool isCheckedOut = await _cartRepo.DoCheckout(model);
            if (ModelState.IsValid)
                return RedirectToAction(nameof(OrderSuccess));

            return RedirectToAction(nameof(OrderFailure));

        }

        public IActionResult OrderSuccess(Order model)
        {
            var userId = User.Identity.Name.ToLower();  // Obter o ID do usuário logado
            var cartItems = _cartRepo.GetCartItemCount();  // Obter os itens do carrinho
            var finalPrice = model.FinalPrice;
            var order = new Order()
            {
                UserId = userId,
                CreateDate = DateTime.Now,
                FinalPrice = finalPrice,
            };

            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }

    }
}
