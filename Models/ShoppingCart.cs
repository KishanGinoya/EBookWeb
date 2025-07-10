using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingWebUI.Models
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public bool isDeleted { get; set; } = false;
        public IEnumerable<CartDetail> CartDetails { get; set; }
    }
}
