using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingWebUI.Controllers
{
    [Authorize(Roles=nameof(Roles.Admin))]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepository.GetGenres();
            return View(genres);
        }
        public async Task<IActionResult> AddGenre()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddGenre(GenreDTO genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }
            try
            {
                var genreToAdd=new Genre { GenreName=genre.GenreName,Id=genre.Id };
                await _genreRepository.AddGenre(genreToAdd);
                TempData["successMessage"] = "Genre added successfully";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                TempData["errorMessage"] = "Something went wrong while adding genre";
                return View(genre);
            }
        }
        public async Task<IActionResult> UpdateGenre(int id)
        {
            var genre = await _genreRepository.GetGenreById(id);
            if (genre == null)
            {
                throw new InvalidOperationException("Genre not found");
            }
            var genreToUpdate = new GenreDTO
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };
            return View(genreToUpdate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGenre(GenreDTO genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }
            try
            {
                var genreToUpdate = new Genre { Id = genre.Id, GenreName = genre.GenreName };
                await _genreRepository.UpdateGenre(genreToUpdate);
                TempData["successMessage"] = "Genre updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "Something went wrong while updating genre";
                return View(genre);
            }
        }
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepository.GetGenreById(id);
            if (genre == null)
            {
                throw new InvalidOperationException("Genre not found");
            }
            await _genreRepository.DeleteGenre(genre);
            return RedirectToAction(nameof(Index));
        }
    }
}
