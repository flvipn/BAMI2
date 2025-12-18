using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Iepan_Flaviu_Lab2.Data;
using Iepan_Flaviu_Lab2.Models;

namespace Iepan_Flaviu_Lab2.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        private SelectList GetAuthorsSelectList(object? selected = null)
        {
            var items = _context.Author
                .Select(a => new { a.ID, FullName = a.FirstName + " " + a.LastName })
                .OrderBy(a => a.FullName)
                .ToList();
            return new SelectList(items, "ID", "FullName", selected);
        }

        private SelectList GetGenresSelectList(object? selected = null)
        {
            return new SelectList(_context.Genre.OrderBy(g => g.Name).ToList(), "ID", "Name", selected);
        }

        // GET: Books
        public async Task<IActionResult> Index(string sortOrder, string? searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";

            // Keep the current filter so the textbox is preserved
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Book> booksQuery = _context.Book
                .Include(b => b.AuthorRef)
                .Include(b => b.Genre);

            // Apply filtering by title if provided
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Title);
                    break;
                case "Price":
                    booksQuery = booksQuery.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Price);
                    break;
                case "Author":
                    // order by author's last name then first name (nulls last)
                    booksQuery = booksQuery.OrderBy(b => b.AuthorRef!.LastName).ThenBy(b => b.AuthorRef!.FirstName);
                    break;
                case "author_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.AuthorRef!.LastName).ThenByDescending(b => b.AuthorRef!.FirstName);
                    break;
                default:
                    booksQuery = booksQuery.OrderBy(b => b.Title);
                    break;
            }

            var model = await booksQuery
                .Select(b => new BookViewModel
                {
                    ID = b.ID,
                    Title = b.Title,
                    Price = b.Price,
                    FullName = b.AuthorRef != null ? b.AuthorRef.FirstName + " " + b.AuthorRef.LastName : "(no author)"
                })
                .ToListAsync();

            return View(model);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Book
                .Include(b => b.Genre)
                .Include(b => b.AuthorRef)
                .Include(b => b.Orders)
                    .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["GenreID"] = GetGenresSelectList();
            ViewData["AuthorID"] = GetAuthorsSelectList();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Price,GenreID,AuthorID")] Book book)
        {    // if model binding failed, re-populate dropdowns and return view
            if (!ModelState.IsValid)
            {
                ViewData["GenreID"] = GetGenresSelectList(book.GenreID);
                ViewData["AuthorID"] = GetAuthorsSelectList(book.AuthorID);
                return View(book);
            }

            try
            {
                // model binder already created 'book' from form values (ID is omitted so DB will set it)
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Safe user-facing message; consider logging the exception (ILogger) for diagnostics
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact the administrator.");
            }

            // if we got here, something failed — re-populate dropdowns and show form with validation message
            ViewData["GenreID"] = GetGenresSelectList(book.GenreID);
            ViewData["AuthorID"] = GetAuthorsSelectList(book.AuthorID);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Book.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["GenreID"] = GetGenresSelectList(book.GenreID);
            ViewData["AuthorID"] = GetAuthorsSelectList(book.AuthorID);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Book.FirstOrDefaultAsync(s => s.ID == id);
            if (bookToUpdate == null)
            {
                return NotFound();
            }

            // Only update these properties to avoid overposting
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "",
                b => b.Title,
                b => b.Price,
                b => b.GenreID,
                b => b.AuthorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact the administrator.");
                }
            }

            // Re-populate dropdowns and return the view with the entity including any attempted changes
            ViewData["GenreID"] = GetGenresSelectList(bookToUpdate.GenreID);
            ViewData["AuthorID"] = GetAuthorsSelectList(bookToUpdate.AuthorID);
            return View("Edit", bookToUpdate);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Book
                .Include(b => b.Genre)
                .Include(b => b.AuthorRef)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
