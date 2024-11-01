using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcoImpulse.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int produtoId, int qty)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not logged in");

            var cart = await GetCart(userId) ?? new ShoppingCart { UserId = userId };
            if (cart.Id == 0) // If cart is new, add to DB
            {
                await _db.ShoppingCarts.AddAsync(cart);
                await _db.SaveChangesAsync();
            }

            var cartItem = await _db.CartDetails
                .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.ProdutoId == produtoId);
            if (cartItem != null)
            {
                cartItem.Quantity += qty;
            }
            else
            {
                var produto = await _db.Produtos.FindAsync(produtoId);
                cartItem = new CartDetail
                {
                    ProdutoId = produtoId,
                    ShoppingCartId = cart.Id,
                    Quantity = qty,
                    UnitPrice = produto.Price
                };
                await _db.CartDetails.AddAsync(cartItem);
            }

            await _db.SaveChangesAsync();
            return await GetCartItemCount(userId);
        }

        public async Task<int> RemoveItem(int produtoId)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not logged in");

            var cart = await GetCart(userId);
            if (cart == null)
                throw new InvalidOperationException("Invalid cart");

            var cartItem = await _db.CartDetails
                .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.ProdutoId == produtoId);
            if (cartItem == null)
                throw new InvalidOperationException("No items in cart");

            if (cartItem.Quantity == 1)
                _db.CartDetails.Remove(cartItem);
            else
                cartItem.Quantity -= 1;

            await _db.SaveChangesAsync();
            return await GetCartItemCount(userId);
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Invalid user id");

            return await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Produto)
                .ThenInclude(a => a.Estoque)
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Produto)
                .ThenInclude(a => a.Genero)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            return await _db.CartDetails
                .CountAsync(cd => cd.ShoppingCart.UserId == userId);
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged in");
                }

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    throw new InvalidOperationException("Invalid cart");
                }

                var cartDetails = await _db.CartDetails
                  .Where(a => a.ShoppingCartId == cart.Id)
                  .ToListAsync();
                if (!cartDetails.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                var pendingRecord = await _db.orderStatuses.FirstOrDefaultAsync(s => s.StatusName == "Pending");
                if (pendingRecord == null)
                {
                    throw new InvalidOperationException("Order status does not have Pending status");
                }

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };

                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                foreach (var item in cartDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProdutoId = item.ProdutoId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    await _db.OrderDetails.AddAsync(orderDetail);

                    var estoque = await _db.Stocks.FirstOrDefaultAsync(a => a.ProdutoId == item.ProdutoId);
                    if (estoque == null)
                    {
                        throw new InvalidOperationException("Estoque is null");
                    }

                    if (item.Quantity > estoque.Quantity)
                    {
                        throw new InvalidOperationException("Not enough stock for product: " + item.Produto.ProdutoName); // Include product name in error message
                    }
                    estoque.Quantity -= item.Quantity;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during checkout: {ex.Message}");  // Log the exception
                await transaction.RollbackAsync();
                return false;
            }
        }

        

            public async Task ClearCart()
            {
                var userId = GetUserId();
                var cart = await _db.ShoppingCarts.Include(c => c.CartDetails)
                                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    _db.CartDetails.RemoveRange(cart.CartDetails); // Remove os detalhes do carrinho
                    _db.ShoppingCarts.Remove(cart); // Remove o carrinho
                    await _db.SaveChangesAsync();
                }
           
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }
    }
}
