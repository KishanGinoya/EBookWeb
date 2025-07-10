namespace BookShoppingWebUI.Models.DTOs
{
    public class BookDisplayModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string sTerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
