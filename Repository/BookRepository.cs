
using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddBook(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBook(Book book)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _db.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _db.Books.Include(b => b.Genre).ToListAsync();
        }

        public async Task UpdateBook(Book book)
        {
            _db.Books.Update(book);
            await _db.SaveChangesAsync();
        }
    }
}
