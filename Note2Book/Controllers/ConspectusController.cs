using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers;

[Route("[controller]")]
public class ConspectusController : Controller
{
    private readonly DataContext _context;
    public ConspectusController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var folders = await _context.Folders
            .Select(c => new Folder
            {
                Id = c.Id,
                Text = c.Text,
                Notes = c.Notes.Select(x => new Note
                {
                    Id = x.Id,
                    Text = x.Text,
                    Author = new User
                    {
                        Id = x.Author.Id,
                        Name = x.Author.Name,
                    },
                    DateTime = default
                }).ToList(),
                User = new User
                {
                    Id = c.User.Id,
                    Name = c.User.Name,
                },
            })
            .ToListAsync();
        return View(folders);
    }

    // [HttpGet("{id:int}")]
    // public async Task<IActionResult> Book(int id)
    // {
    //     var book = await _context.Books
    //         .Where(c => c.Id == id)
    //         .Select(c => new Book
    //         {
    //             Id = c.Id,
    //             Title = c.Title,
    //             Description = c.Description,
    //             Genres = c.Genres.Select(x => new Genre()
    //             {
    //                 Id = x.Id,
    //                 Title = x.Title
    //             }).ToList(),
    //             CreateAt = c.CreateAt,
    //             Image = c.Image,
    //             Chapters = c.Chapters.Select(x => new Chapter()
    //             {
    //                 Id = x.Id, 
    //                 Title = x.Title
    //             }).ToList(),
    //             Author = new Author()
    //             {
    //                 Id = c.Author.Id,
    //                 Name = c.Author.Name,
    //                 LastName = c.Author.LastName,
    //             }
    //         })
    //         .FirstOrDefaultAsync();
    //     
    //     return View(book);
    // }
}