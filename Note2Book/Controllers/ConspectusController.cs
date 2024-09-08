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
                ImageUrl = c.ImageUrl,
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
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
        return View(folders);
    }

}