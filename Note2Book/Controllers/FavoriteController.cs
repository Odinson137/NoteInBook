using Microsoft.AspNetCore.Mvc;
using Note2Book.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Note2Book.Data;

namespace Note2Book.Controllers
{
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
        public IActionResult AddToFavorites(int bookId)
        {
            var userIdstring = User.Identity.Name; // Получите идентификатор текущего пользователя
            var userIdstring = User.Identity.Name; // Получите идентификатор текущего пользователя
            if (userId != null)
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
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Book");
        }
    }
}
