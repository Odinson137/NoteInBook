using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Interfaces;
using Note2Book.Models;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;
    private readonly IActivityService _activityService;
    public HomeController(DataContext context, IActivityService activityService)
    {
        _context = context;
        _activityService = activityService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdCookie = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdCookie == null)
        {
            return RedirectToAction("Index", "Book");
            
        }
    
        var userId = int.Parse(userIdCookie);
    
        var favoriteBooks = await _context.Favorites
            .Where(c => c.UserBook == UserBook.Favorite)
            .Include(f => f.Book)
            .ThenInclude(b => b.Author) 
            .Where(f => f.User.Id == userId)
            .Select(f => new BookViewModel
            {
                Title = f.Book.Title,
                Image = f.Book.Image,
                Author = f.Book.Author
            }).ToListAsync();

        var bookWithUserStatus = await _context.Favorites
            .Where(c => c.UserBook != UserBook.Favorite)
            .Include(f => f.Book)
            .ThenInclude(b => b.Author) 
            .Where(f => f.User.Id == userId)
            .Select(f => new Favorite
            {
                UserBook = f.UserBook,
                Book = new Book
                {
                    Id = f.Book.Id,
                    Title = f.Book.Title,
                    Image = f.Book.Image,
                    Author = f.Book.Author
                }
            }).ToListAsync();

        var activities = _activityService.GetDayActivities(userId);
        var homeViewModel = new HomeViewModel
        {
            FavoriteBooks = favoriteBooks,
            BookStatus = bookWithUserStatus,
            Activities = activities
        };
        return View(homeViewModel);
    }
    
    // [HttpPost]
    // [Route("api/books/update-status")]
    // public IActionResult UpdateBookStatuses([FromBody] List<BookStatusUpdateViewModel> updates)
    // {
    //     foreach (var update in updates)
    //     {
    //         var book = _context.Books.FirstOrDefault(b => b.Id == update.Id);
    //         if (book != null)
    //         {
    //             book.Status = update.Status;
    //         }
    //     }
    //
    //     _context.SaveChanges();
    //     return Ok();
    // }
}