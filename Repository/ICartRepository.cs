using BookShoppingWebUI.Models.DTOs;

namespace BookShoppingWebUI.Repository
{
    public interface ICartRepository
    {
        Task<ShoppingCart> GetUserCart();
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<int> GetCartItemCount(string userId = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(CheckoutModel model);
    }
}