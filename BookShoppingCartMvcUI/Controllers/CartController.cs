using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoImpulse.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IUserOrderRepository _orderRepo;
        private readonly ApplicationDbContext _context;

        public CartController(ICartRepository cartRepo, IUserOrderRepository orderRepo, ApplicationDbContext context)
        {
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _context = context;
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
            // Add more detailed model validation
            if (!ModelState.IsValid)
            {
                // Log specific validation errors
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Log validation errors
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error}");
                }

                return View(model);
            }

            try
            {
                var orderId = await CreateOrder(model);

                if (orderId == null)
                {
                    // Log more detailed error information
                    Console.WriteLine("Order creation failed. Redirecting to OrderFailure.");
                    return RedirectToAction(nameof(OrderFailure));
                }

                // Clear the cart after successful order
                await _cartRepo.ClearCart();

                return RedirectToAction(nameof(OrderSuccess), new { orderId });
            }
            catch (Exception ex)
            {
                // More comprehensive error logging
                Console.WriteLine($"Checkout process failed: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return RedirectToAction(nameof(OrderFailure));
            }

            async Task<int?> CreateOrder(CheckoutModel model)
            {
                // Validate user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("No user ID found. Cannot create order.");
                    return null;
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Retrieve cart items
                        var cartItems = await _cartRepo.GetUserCart();

                        // Additional validation
                        if (cartItems?.CartDetails == null || !cartItems.CartDetails.Any())
                        {
                            Console.WriteLine("Cart is empty. Cannot create order.");
                            return null;
                        }

                        var order = new Order
                        {
                            UserId = userId,
                            Name = model.Name,
                            Email = model.Email,
                            MobileNumber = model.MobileNumber,
                            Address = model.Address,
                            PaymentMethod = model.PaymentMethod,
                            CreateDate = DateTime.UtcNow,
                            IsPaid = false
                        };

                        // Ensure order details are properly created
                        order.OrderDetail = cartItems.CartDetails.Select(item => new OrderDetail
                        {
                            ProdutoId = item.ProdutoId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            OrderId = order.Id  // Ensure this is set correctly
                        }).ToList();

                        // Explicitly add and save the order
                        _context.Orders.Add(order);
                        var saveResult = await _context.SaveChangesAsync();

                        Console.WriteLine($"Order saved. Entities saved: {saveResult}");

                        int orderId = order.Id;

                        await transaction.CommitAsync();
                        return orderId;
                    }
                    catch (DbUpdateException ex)
                    {
                        // More detailed error logging
                        Console.WriteLine($"Database Update Error: {ex.Message}");
                        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                        await transaction.RollbackAsync();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected Order Creation Error: {ex.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                        await transaction.RollbackAsync();
                        return null;
                    }
                }
            }
        }

       
        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
