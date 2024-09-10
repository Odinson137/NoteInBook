using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers;

[Route("[controller]")]
[Authorize]
public class NoteController : Controller
{
    private readonly DataContext _context;
    
    public NoteController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int folderId, string? search)
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
        
        var query = _context.Folders
            .Where(c => c.Id == folderId)
            .Where(c=>c.User.Id == userId)
            .SelectMany(c => c.Notes)
            .Select(c => new Note
            {
                Id = c.Id,
                Title = c.Title,
                Text = c.Text,
                Author = new User
                {
                    Id = c.Author.Id,
                    Name = c.Author.Name,
                },
                DateTime = c.DateTime
            });

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Title.Contains(search));
        }
        
        var folders = await query.OrderByDescending(c => c.DateTime).ToListAsync();
        
        ViewBag.FolderId = folderId;
        ViewBag.SearchQuery = search;
        
        return View(folders);
    }
    
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
       var note = await _context.Notes
            .Select(c => new Note
                {
                    Id = c.Id,
                    Title = c.Title,
                    Text = c.Text,
                    Author = new User
                    {
                        Id = c.Author.Id,
                        Name = c.Author.Name,
                    },
                    DateTime = c.DateTime
                })
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }

    // POST: Note/Edit/5
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Note updatedNote)
    {
        if (id != updatedNote.Id)
        {
            return BadRequest();
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        note.Title = updatedNote.Title;
        note.Text = updatedNote.Text;
        note.DateTime = DateTime.Now; // Обновляем дату изменения

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Notes.Any(n => n.Id == updatedNote.Id))
            {
                return NotFound();
            }
            throw;
        }

        var folderId = await _context.Folders.Where(c => c.Notes.Any(x => x.Id == note.Id)).Select(c => c.Id).FirstAsync();

        return RedirectToAction(nameof(Index), new {folderId});
    }
    
    [HttpGet("Create")]
    public IActionResult Create(int folderId)
    {
        ViewBag.FolderId = folderId;
        return View();
    }

    // POST: Note/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int folderId, Note newNote)
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
        ViewBag.FolderId = folderId;
        
        var user = await _context.Users.FirstAsync(c => c.Id == userId);
        var folder = await _context.Folders.Include(c => c.Notes).FirstAsync(c => c.Id == folderId);
        
        newNote.Author = user;
        
        folder.Notes.Add(newNote);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Edit), new { newNote.Id});
        
    }
}