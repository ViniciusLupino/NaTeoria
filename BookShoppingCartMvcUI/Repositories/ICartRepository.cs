namespace BookShoppingCartMvcUI.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int produtoId, int qty);
        Task<int> RemoveItem(int produtoId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(CheckoutModel model);
    }
}
