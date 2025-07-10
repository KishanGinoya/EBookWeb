using System.ComponentModel.DataAnnotations;

namespace BookShoppingWebUI.Models.DTOs
{
    public class CheckoutModel
    {
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string? Email { get; set; }

        [Required]
        public string? MobileNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }
}
