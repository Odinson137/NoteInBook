using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        var hasFolder = await _context.Folders.FindAsync(folderId);
        if (hasFolder == null)
        {
            return RedirectToAction("Index", "Conspectus");
        }
        
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
        ViewBag.FolderName = hasFolder.Text;
        
        return View(folders);
    }
    
    [Route("List")]
    public async Task<IActionResult> List(int bookId)
    {
        // Извлекаем userId из куки
        var userIdCookie = Request.Cookies["UserId"];
        if (userIdCookie == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var userId = int.Parse(userIdCookie);
        
        var query = await _context.Citations
            .Where(c => bookId == c.Chapter.Book.Id)
            .Where(c=>c.Author.Id == userId)
            .Select(c => new CitationViewModel
            {
                Id = c.Id,
                Text = c.Text,
                Comment = c.Comment,
                DateTime = c.DateTime,
            }).ToListAsync();

        return View(query);
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
    
    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _context.Notes.FindAsync(id);

        var folderId = await _context.Folders
            .Where(c => c.Notes.Any(x => x.Id == id))
            .Select(c => c.Id)
            .FirstAsync();
        
        if (note != null)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction(nameof(Index), new {folderId});
    }
}