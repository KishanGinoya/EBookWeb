using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingWebUI.Models.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? BookName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? AuthorName { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }

        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
