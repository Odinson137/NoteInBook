using Microsoft.AspNetCore.Mvc;
using Note2Book.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

[Route("[controller]")]
[Authorize]
public class FavoriteController : Controller
{
    private readonly DataContext _context;

    public FavoriteController(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddToFavorites(int bookId)
    {
        var userIdCookie = Request.Cookies["UserId"];
        var userId = int.Parse(userIdCookie!);

        var favoriteDb = await _context.Favorites.FirstOrDefaultAsync(c => c.Book.Id == bookId && c.User.Id == userId && c.UserBook == UserBook.Favorite);
        if (favoriteDb == null)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);

            if (user != null && book != null)
            {
                var favorite = new Favorite
                {
                    Book = book,
                    User = user
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            } 
            
            return Json(new { success = false });
        }

        _context.Favorites.Remove(favoriteDb);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFavoriteBooks()
    {
        var userIdCookie = Request.Cookies["UserId"];
        if (userIdCookie == null)
        {
            return Unauthorized();
        }
        var userId = int.Parse(userIdCookie);
    
        var favoriteBooks = await _context.Favorites.Where(c => c.UserBook == UserBook.Favorite)
            .Include(f => f.Book)
            .ThenInclude(b => b.Author) 
            .Where(f => f.User.Id == userId)
            .Select(f => new BookViewModel
            {
                Title = f.Book.Title,
                Image = f.Book.Image,
                Author = f.Book.Author
            }).ToListAsync();
    
        return Json(favoriteBooks);
    }

}