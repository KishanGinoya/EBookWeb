using BookShoppingWebUI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class CartRepository:ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if(userId == null)
            {
                throw new Exception("User not found");
            }
            var shoppingCarts = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Book)
                .ThenInclude(a=>a.Stock)
                .Include(a=>a.CartDetails)
                .ThenInclude(a => a.Book)
                .ThenInclude(a => a.Genre)
                .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCarts;
        }

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userID = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                
                if (string.IsNullOrEmpty(userID))
                {
                    throw new Exception("User not found");
                }
                var cart = await GetCart(userID);
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userID
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                //cart details section
                var cartItem = await _db.CartDetails.FirstOrDefaultAsync(c => c.BookId == bookId && c.ShoppingCartId == cart.Id);
                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book= await _db.Books.FindAsync(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }
                await _db.SaveChangesAsync();
                transaction.Commit();
                
            }
            catch (Exception ex)
            {
                
            }
            var cartItemCount = await GetCartItemCount(userID);
            return cartItemCount;
        }
        public async Task<int> RemoveItem(int bookId)
        {
            string userID = GetUserId();
            //using var transaction = _db.Database.BeginTransaction();
            try
            {
                
                if (string.IsNullOrEmpty(userID))
                {
                    throw new Exception("User not found");
                }
                var cart = await GetCart(userID);
                if (cart == null)
                {
                    throw new Exception("Cart not found");
                }

                //cart details section
                var cartItem = _db.CartDetails.FirstOrDefault(c => c.BookId == bookId && c.ShoppingCartId == cart.Id);
                if (cartItem == null)
                {
                    throw new Exception("Cart item not found");
                }
                else if (cartItem.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity -= 1;
                }
                _db.SaveChanges();
                //transaction.Commit();
                
            }
            catch (Exception ex)
            {
                
            }
            var cartItemCount = await GetCartItemCount(userID);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.UserId == userId);
            return cart;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cart.Id==cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                     ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if(string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User not found");
                }
                var cart=await GetCart(userId);
                if(cart == null)
                {
                    throw new Exception("Cart not found");
                }
                var cartDetails = await _db.CartDetails
                                         .Where(a => a.ShoppingCartId == cart.Id).ToListAsync();
                if(cartDetails.Count == 0)
                {
                    throw new Exception("Cart is empty");
                }
                var pendingRecord = _db.OrderStatus.FirstOrDefault(s => s.StatusName == "Pending");
                if(pendingRecord == null)
                {
                    throw new Exception("Pending order status not found");
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
                    IsPaid =false,
                    OrderStatusId = pendingRecord.Id
                };
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
                foreach(var item in cartDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId=item.BookId,
                        OrderId=order.Id,
                        Quantity=item.Quantity,
                        UnitPrice= item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                    var stocks=await _db.Stocks.FirstOrDefaultAsync(s => s.BookId == item.BookId);
                    if(stocks == null)
                    {
                        throw new Exception("Stock not found for book id: " + item.BookId);
                    }
                    if(stocks.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient stock for book id: " + item.BookId);
                    }
                    stocks.Quantity -= item.Quantity;
                }
                await _db.SaveChangesAsync();

                //remove cart details
                _db.CartDetails.RemoveRange(cartDetails);
                await _db.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
