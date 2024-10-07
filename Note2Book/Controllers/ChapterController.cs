using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Note2Book.Models;
using Note2Book.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
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
                .Select(c => c.Id)
                .FirstOrDefault();

            var previousChapter = _context.Chapters
                .Where(c => c.Book.Id == chapter.Book.Id && c.Number < chapter.Number)
                .OrderByDescending(c => c.Number)
                .Select(c => c.Id)
                .FirstOrDefault();

            var citations = new List<CitationViewModel>();
            var userIdCookie = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdCookie != null)
            {
                var userId = int.Parse(userIdCookie);
            
                citations = _context.Citations
                    .Where(c => c.Author.Id == userId)
                    .Where(c => c.Chapter.Id == chapterId &&
                                c.Start >= startIndex &&
                                c.End <= startIndex + pageSize)
                    .Select(c => new CitationViewModel
                    {
                        Id = c.Id,
                        Text = c.Text,
                        Comment = c.Comment,
                        Start = c.Start,
                        End = c.End
                    })
                    .ToList();
            }

            var model = new ChapterViewModel
            {
                Chapter = chapter,
                PageText = pageText,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                NextChapter = nextChapter,
                PreviousChapter = previousChapter,
                Citations = citations
            };

            return View(model);
        }


    }
}
