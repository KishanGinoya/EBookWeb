using System.Collections;
using System.Diagnostics;
using BookShoppingWebUI.Models;
using BookShoppingWebUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger,IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm="",int genreId=0)
        {
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm, genreId);
            IEnumerable<Genre> genre = await _homeRepository.Genres();
            BookDisplayModel booksGenres=new BookDisplayModel
            {
                Books = books,
                Genres = genre,
                GenreId = genreId,
                sTerm = sterm
            };
            return View(booksGenres);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
