using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Models;

namespace Note2Book.Controllers;

public class CitationController : Controller
{
    private readonly DataContext _context;

    public CitationController(DataContext context)
    {
        _context = context;
    }
  

    [HttpPost]
    public async Task<IActionResult> Create(int chapterId, string text, int start, int end)
    {
        // Извлекаем userId из куки
        var userIdCookie = Request.Cookies["UserId"];
        if (userIdCookie == null)
        {
            return Unauthorized("Пользователь не авторизован.");
        }

        int userId = int.Parse(userIdCookie);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Unauthorized("Пользователь не найден.");
        }

        // Извлекаем главу по chapterId
        var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
        {
            return NotFound("Глава не найдена.");
        }

        // Создаем новую цитату
        var newCitation = new Citation
        {
            Text = text,
            Start = start,
            End = end,
            Author = user,
            Chapter = chapter
        };

        // Добавляем новую цитату в базу данных
        _context.Citations.Add(newCitation);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Цитата успешно добавлена" });
    }

}