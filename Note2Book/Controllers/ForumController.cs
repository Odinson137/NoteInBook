using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly DataContext _context;

        public ForumController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .Include(f => f.Book)
                .Include(f => f.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(f => f.BookId == bookId);

            if (forum == null)
            {
                forum = new Forum { BookId = bookId };
                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();
                forum.Book = book;
            }

            return View(forum);
        }
    }
}