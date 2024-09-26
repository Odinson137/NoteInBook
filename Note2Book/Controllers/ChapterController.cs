using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Models;
using Note2Book.ViewModels;
using System;
using System.Linq;
using Note2Book.Data;

namespace Note2Book.Controllers
{
    public class ChapterController : Controller
    {
        private readonly DataContext _context;

        public ChapterController(DataContext context)
        {
            _context = context;
        }

        public IActionResult ReadChapter(int chapterId, int pageNumber = 1)
        {
            var chapter = _context.Chapters
                                  .Include(c => c.Book)
                                  .FirstOrDefault(c => c.Id == chapterId);

            if (chapter == null)
            {
                return NotFound();
            }

            const int pageSize = 5000;
            var totalCharacters = chapter.Text.Length;
            var totalPages = (int)Math.Ceiling((double)totalCharacters / pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return BadRequest("Неверный номер страницы.");
            }

            var startIndex = (pageNumber - 1) * pageSize;
            var pageText = chapter.Text.Substring(startIndex, Math.Min(pageSize, totalCharacters - startIndex));

            var nextChapter = _context.Chapters
                                      .Where(c => c.Book.Id == chapter.Book.Id && c.Number > chapter.Number)
                                      .OrderBy(c => c.Number)
                                      .FirstOrDefault();

            var previousChapter = _context.Chapters
                                           .Where(c => c.Book.Id == chapter.Book.Id && c.Number < chapter.Number)
                                           .OrderByDescending(c => c.Number)
                                           .FirstOrDefault();

            var model = new ChapterViewModel
            {
                Chapter = chapter,
                PageText = pageText,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                NextChapter = nextChapter,
                PreviousChapter = previousChapter
            };

            return View(model);
        }
    }
}
