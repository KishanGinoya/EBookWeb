
using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _db;

        public GenreRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddGenre(Genre genre)
        {
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteGenre(Genre genre)
        {
            _db.Genres.Remove(genre);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreById(int id)
        {
            return await _db.Genres.FindAsync(id);
        }

        public async Task UpdateGenre(Genre genre)
        {
            _db.Genres.Update(genre);
            await _db.SaveChangesAsync();
        }
    }
}
