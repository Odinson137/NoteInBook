using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers;

[Route("[controller]")]
[Authorize]
public class ConspectusController : Controller
{
    private readonly DataContext _context;
    public ConspectusController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
         // Извлекаем userId из куки
            var userIdCookie = Request.Cookies["UserId"];
            if (userIdCookie == null)
            {
                // Если куки нет, перенаправляем пользователя на страницу логина
                return RedirectToAction("Login", "User");
            }
        
            // Преобразуем userId из строки в int (если используется int в базе данных)
            int userId = int.Parse(userIdCookie);
            
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
            .Where(c => c.User.Id == userId)
            .ToListAsync();
        return View(folders);
    }
    [HttpGet]
    [Route("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(Folder model)
    {
        var userIdCookie = Request.Cookies["UserId"];
        if (userIdCookie == null) return RedirectToAction("Login", "User");

        int userId = int.Parse(userIdCookie);

        var user = await _context.Users.FirstAsync(u => u.Id == userId);
        
        var folder = new Folder
        {
            Text = model.Text,
            ImageUrl = model.ImageUrl ?? "images/folder.png", // Добавьте поле для ввода URL изображения
            User = user,
        };

        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var folder = await _context.Folders.FindAsync(id);
        if (folder == null)
        {
            return NotFound();
        }

        // Удаляем папку
        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

}