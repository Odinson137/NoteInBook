using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note2Book.Data;
using Note2Book.Models;
using System.Threading.Tasks;

namespace Note2Book.Controllers
{
    public class CommentController : Controller
    {
        private readonly DataContext _context;

        public CommentController(DataContext context)
        {
            _context = context;
        }

        [Authorize]  // Только авторизованные пользователи могут добавлять комментарии
        [HttpPost]
        public async Task<IActionResult> AddComment(Test test)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (userIdCookie == null)
            {
                // Если куки нет, перенаправляем пользователя на страницу логина
                return RedirectToAction("Login", "User");
            }
        
            // Преобразуем userId из строки в int (если используется int в базе данных)
            int userId = int.Parse(userIdCookie);
            var book = await _context.Books.FindAsync(test.bookId);

           

            var newComment = new BookComment
            {
                Author = _context.Users.Find(userId),
                Text = test.commentText,
                Book = book
            };

            _context.BookComments.Add(newComment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Комментарий успешно добавлен" });
        }
    }
}

public class Test
{
    public int bookId { get; set; }
    public string commentText { get; set; }
}