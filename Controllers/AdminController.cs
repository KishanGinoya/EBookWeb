using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingWebUI.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))] 

    public class AdminController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;

        public AdminController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderRepository.UserOrders(true);
            return View(orders);
        }
        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderRepository.TogglePaymentStatus(orderId);
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("AllOrders");
        }
        public async Task<IActionResult> UpdatePaymentStatus(int orderId)
        {
            var order = await _userOrderRepository.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }
            var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem
                {
                    Text = orderStatus.StatusName,
                    Value = orderStatus.Id.ToString(),
                    Selected = order.OrderStatusId == orderStatus.Id
                };
            });
            var data = new UpdateOrderStatusModel
            {
                OrderId = order.Id,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(UpdateOrderStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                model.OrderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus => new SelectListItem
                {
                    Text = orderStatus.StatusName,
                    Value = orderStatus.Id.ToString(),
                    Selected = model.OrderStatusId == orderStatus.Id
                });
                return View(model);
            }

            try
            {
                await _userOrderRepository.ChangeOrderStatus(model);
                TempData["msg"] = "Updated order status successfully";
                return RedirectToAction(nameof(AllOrders));
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Failed to update order status";
                // Re-populate dropdown in case of failure
                model.OrderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus => new SelectListItem
                {
                    Text = orderStatus.StatusName,
                    Value = orderStatus.Id.ToString(),
                    Selected = model.OrderStatusId == orderStatus.Id
                });
                return View(model);
            }
        }
        public IActionResult Dashboard()
        {
            var stats=_userOrderRepository.GetDashboardStats();
            return View(stats);
        }

    }
}
