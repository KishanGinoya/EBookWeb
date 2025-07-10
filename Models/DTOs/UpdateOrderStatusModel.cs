using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingWebUI.Models.DTOs
{
    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }
        [Required]
        public int OrderStatusId { get; set; }
        [BindNever]
        [ValidateNever]
        public IEnumerable<SelectListItem> OrderStatusList { get; set; }
    }
}
