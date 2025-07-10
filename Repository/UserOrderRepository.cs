using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class UserOrderRepository:IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db,IHttpContextAccessor httpContextAccessor,UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public DashboardStatsDTO GetDashboardStats()
        {
            return new DashboardStatsDTO
            {
                TotalBooks = _db.Books.Count(),
                TotalOrders = _db.Orders.Count(),
                TotalGenres = _db.Genres.Count(),
                TotalCheckouts = _db.Orders.Count(o => o.OrderStatus.StatusId>=1 && o.OrderStatus.StatusId <= 3) // customize condition
            };
        }

        public async Task ChangeOrderStatus(UpdateOrderStatusModel model)
        {
            var order=await _db.Orders.FindAsync(model.OrderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            order.OrderStatusId = model.OrderStatusId;
            await _db.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _db.Orders.FindAsync(id);
            
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _db.OrderStatus.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order=await _db.Orders.FindAsync(orderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            order.IsPaid = !order.IsPaid;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
        {
            var orders=_db.Orders
                          .Include(x=>x.OrderStatus)
                          .Include(x=>x.OrderDetails)
                          .ThenInclude(x=>x.Book)
                          .ThenInclude(x=>x.Genre).AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                if(string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User not found");
                }
                orders=orders.Where(a=>a.UserId==userId);
                return await orders.ToListAsync();
            }
            return await orders.ToListAsync();
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
