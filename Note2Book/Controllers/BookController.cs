using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Dto;
using Note2Book.Interfaces;
using Note2Book.Models;
using Note2Book.ViewModels;

namespace Note2Book.Controllers;

[Route("[controller]")]
public class BookController : Controller
{
    private readonly DataContext _context;
    private readonly IBookElasticService _bookElasticService;
    public BookController(DataContext context, IBookElasticService bookElasticService)
    {
        _context = context;
        _bookElasticService = bookElasticService;
    }
       
    [HttpGet]
    public async Task<IActionResult> Index(string searchString)
    {
        // Извлекаем все книги
        var booksQuery = _context.Books.AsQueryable();

        var stopwatch = Stopwatch.StartNew();

        // Проверяем, если в строке поиска что-то введено
        if (!string.IsNullOrEmpty(searchString))
        {
            // Поиск по названию книги, автору и жанру
            // booksQuery = booksQuery.Where(b =>
            //     b.Title.Contains(searchString) ||
            //     b.Author.Name.Contains(searchString) ||
            //     b.Author.LastName.Contains(searchString) ||
            //     b.Genres.Any(g => g.Title.Contains(searchString))
            // );

            var response = await _bookElasticService.SearchBooksAsync(searchString);
            booksQuery = booksQuery.Where(book => response.Contains(book.Id));
        }
        
        var userIdCookie = Request.Cookies["UserId"] ?? "0";
        var userId = int.Parse(userIdCookie);
        var userFavoritesIds = await _context.Favorites.Where(c => c.UserBook == UserBook.Favorite)
            .Where(c => c.User.Id == userId)
            .Select(c => c.Book.Id)
            .ToListAsync();

        // Извлекаем данные для отображения
        var books = await booksQuery
            .Select(c => new BookViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Genres = c.Genres.Select(x => new Genre { Title = x.Title }).ToList(),
                CreateAt = c.CreateAt,
                Image = c.Image,
                Chapters = c.Chapters.Select(x => new Chapter { Title = x.Title }).ToList(),
                Author = new Author
                {
                    Name = c.Author.Name,
                    LastName = c.Author.LastName,
                },
                IsLiked = userFavoritesIds.Contains(c.Id)
            })
            .ToListAsync();

        stopwatch.Stop();

        // Выводим результат
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");

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
                Genres = c.Genres.Select(x => new Genre
                {
                    Id = x.Id,
                    Title = x.Title
                }).ToList(),
                CreateAt = c.CreateAt,
                Image = c.Image,
                Chapters = c.Chapters.Select(x => new Chapter
                {
                    Id = x.Id,
                    Title = x.Title
                }).ToList(),
                Author = new Author
                {
                    Id = c.Author.Id,
                    Name = c.Author.Name,
                    LastName = c.Author.LastName,
                },
                Storages = c.Storages.Select(x => new Storage
                {
                    Url = x.Url,
                    FileType = x.FileType,
                }).ToList(),
                Comments = c.Comments
                    .OrderByDescending(comment => comment.Id)
                    .Select(comment => new BookComment
                    {
                        Id = comment.Id,
                        Text = comment.Text,
                        Author = new User
                        {
                            Id = comment.Author.Id,
                            Name = comment.Author.Name,
                            Login = comment.Author.Login
                        },
                        CreatedAt = comment.CreatedAt
                    }).ToList()
            })
            .FirstOrDefaultAsync();
        
        return View(book);
    }
}