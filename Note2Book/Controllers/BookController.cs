using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers;

[Route("[controller]")]
public class BookController : Controller
{
    private readonly DataContext _context;
    public BookController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Select(c => new Book
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Genres = c.Genres.Select(x => new Genre() {Title = x.Title}).ToList(),
                CreateAt = c.CreateAt,
                Image = c.Image,
                Chapters = c.Chapters.Select(x => new Chapter() {Title = x.Title}).ToList(),
                Author = new Author()
                {
                    Name = c.Author.Name,
                    LastName = c.Author.LastName,
                }
            })
            .ToListAsync();
        return View(books);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Book(int id)
    {
        var book = await _context.Books
            .Where(c => c.Id == id)
            .Select(c => new Book
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Genres = c.Genres.Select(x => new Genre()
                {
                    Id = x.Id,
                    Title = x.Title
                }).ToList(),
                CreateAt = c.CreateAt,
                Image = c.Image,
                Chapters = c.Chapters.Select(x => new Chapter()
                {
                    Id = x.Id, 
                    Title = x.Title
                }).ToList(),
                Author = new Author()
                {
                    Id = c.Author.Id,
                    Name = c.Author.Name,
                    LastName = c.Author.LastName,
                }
            })
            .FirstOrDefaultAsync();
        
        return View(book);
    }
}