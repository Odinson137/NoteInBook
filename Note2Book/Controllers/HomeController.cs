using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;
    public HomeController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userIdCookie = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdCookie == null)
        {
            return View(new HomeViewModel
            {
                FavoriteBooks = []
            });
        }
        var userId = int.Parse(userIdCookie);
    
        var favoriteBooks = await _context.Favorites
            .Include(f => f.Book)
            .ThenInclude(b => b.Author) 
            .Where(f => f.User.Id == userId)
            .Select(f => new BookViewModel
            {
                Title = f.Book.Title,
                Image = f.Book.Image,
                Author = f.Book.Author
            }).ToListAsync();

        var homeViewModel = new HomeViewModel
        {
            FavoriteBooks = favoriteBooks
        };
        return View(homeViewModel);
    }
}