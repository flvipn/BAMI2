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
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Book
                .Include(b => b.Genre)
                // adjust Include depending on your Book model navigation name:
                .Include(b => b.AuthorRef);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Book
                .Include(b => b.Genre)
                .Include(b => b.AuthorRef)
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
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Price,GenreID,AuthorID")] Book book)
        {
            if (id != book.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Book.Any(e => e.ID == book.ID)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["GenreID"] = GetGenresSelectList(book.GenreID);
            ViewData["AuthorID"] = GetAuthorsSelectList(book.AuthorID);
            return View(book);
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
