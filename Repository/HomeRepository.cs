
using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Book> books = await (from book in _db.Books
                                             join genre in _db.Genres
                                             on book.GenreId equals genre.Id
                                             join stock in _db.Stocks
                                             on book.Id equals stock.BookId 
                                             into book_stocks
                                             from bookWithStock in book_stocks.DefaultIfEmpty()

                                                 //if the search term is empty then display full record otherwise display with bookname and authorname
                                             where (string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().Contains(sTerm) || book.AuthorName.ToLower().Contains(sTerm))
                                                   
                                             select new Book
                                             {
                                                 Id = book.Id,
                                                 BookName = book.BookName,
                                                 AuthorName = book.AuthorName,
                                                 Price = book.Price,
                                                 Image = book.Image,
                                                 GenreId = book.GenreId,
                                                 GenreName = genre.GenreName,
                                                 Quantity=bookWithStock==null?0: bookWithStock.Quantity
                                             }
                ).ToListAsync();
            if (genreId > 0)
            {
                books = books.Where(b => b.GenreId == genreId);
            }
            return books;
        }
    }
}
