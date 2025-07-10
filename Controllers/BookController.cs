using BookShoppingWebUI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingWebUI.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IFileService _fileService;
        public BookController(IBookRepository bookRepository, IGenreRepository genreRepository, IFileService fileService)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetBooks();
            return View(books);
        }

        public async Task<IActionResult> AddBook()
        {
            var genreSelectList = (await _genreRepository.GetGenres()).Select(genre => new
                SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString()
            });
            BookDTO bookToAdd = new() { GenreList = genreSelectList };
            return View(bookToAdd);
        }
        [HttpPost]
        public async Task<IActionResult> AddBook(BookDTO bookToAdd)
        {
            var genreSelectList = (await _genreRepository.GetGenres()).Select(genre => new
                SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString()
            });
            bookToAdd.GenreList = genreSelectList;
            if (!ModelState.IsValid)
            {
                return View(bookToAdd);
            }
            try
            {
                if (bookToAdd.ImageFile != null)
                {
                    if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not be exceed 1 mb");
                    }
                    string[] allowedExtension = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(bookToAdd.ImageFile, allowedExtension);
                    bookToAdd.Image = imageName;
                }
                Book book = new()
                {
                    Id = bookToAdd.Id,
                    BookName = bookToAdd.BookName,
                    AuthorName = bookToAdd.AuthorName,
                    Image = bookToAdd.Image,
                    GenreId = bookToAdd.GenreId,
                    Price = bookToAdd.Price
                };
                await _bookRepository.AddBook(book);
                TempData["successMessage"] = "Book is Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToAdd);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToAdd);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data...";
                return View(bookToAdd);
            }
        }
        public async Task<IActionResult> UpdateBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {
                TempData["errorMessage"] = "Book not found";
                return RedirectToAction(nameof(Index));
            }
            var genreSelectList = (await _genreRepository.GetGenres()).Select(genre => new
                SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == book.GenreId
            });
            BookDTO bookToUpdate = new()
            {
                BookName = book.BookName,
                AuthorName = book.AuthorName,
                Image = book.Image,
                GenreId = book.GenreId,
                Price = book.Price,
                GenreList = genreSelectList
            };
            return View(bookToUpdate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookDTO bookToUpdate)
        {
            var genreSelectList = (await _genreRepository.GetGenres()).Select(genre => new
                SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString()
            });
            bookToUpdate.GenreList = genreSelectList;
            if (!ModelState.IsValid)
            {
                return View(bookToUpdate);
            }
            try
            {
                string oldImage = "";
                if (bookToUpdate.ImageFile != null)
                {
                    if (bookToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not be exceed 1 mb");
                    }
                    string[] allowedExtension = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(bookToUpdate.ImageFile, allowedExtension);
                    oldImage = bookToUpdate.Image;
                    bookToUpdate.Image = imageName;
                }
                Book book = new()
                {
                    Id = bookToUpdate.Id,
                    BookName = bookToUpdate.BookName,
                    AuthorName = bookToUpdate.AuthorName,
                    Image = bookToUpdate.Image,
                    GenreId = bookToUpdate.GenreId,
                    Price = bookToUpdate.Price
                };
                await _bookRepository.UpdateBook(book);
                TempData["successMessage"] = "Book is Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToUpdate);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToUpdate);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data...";
                return View(bookToUpdate);
            }
        }
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookById(id);
                if (book == null)
                {
                    TempData["errorMessage"] = "Book not found";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await _bookRepository.DeleteBook(book);
                    if (!string.IsNullOrEmpty(book.Image))
                    {
                        _fileService.DeleteFile(book.Image);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on deleting data...";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
