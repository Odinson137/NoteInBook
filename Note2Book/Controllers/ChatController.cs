using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Note2Book.Data;
using Note2Book.Hubs;
using Note2Book.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Note2Book.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly DataContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(DataContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? selectedChatId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var chats = await _context.Chats
                .Where(c => c.ChatMembers.Any(u => u.Id == userId))
                .Include(c => c.ChatMembers)
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .ToListAsync();

            ViewBag.SelectedChatId = selectedChatId ?? (chats.Any() ? chats.First().Id : 0);
            return View(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatEdit model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new { success = false, message = "Название чата обязательно." });
            }

            var chat = new Chat
            {
                Title = model.Title,
                Description = model.Description,
                ChatMembers = new List<User> { user }
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ChatCreated", chat.Id, chat.Title);
            return Json(new { success = true, chatId = chat.Id, title = chat.Title });
        }

        [HttpPost]
        public async Task<IActionResult> EditChat([FromBody] ChatEdit model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var chat = await _context.Chats
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync(c => c.Id == model.ChatId && c.ChatMembers.Any(u => u.Id == userId));

            if (chat == null)
            {
                return Json(new { success = false, message = "Чат не найден или доступ запрещён." });
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return Json(new { success = false, message = "Название чата обязательно." });
            }

            chat.Title = model.Title;
            chat.Description = model.Description;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"Chat_{model.ChatId}").SendAsync("ChatUpdated", model.ChatId, chat.Title);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChat([FromBody] ChatDelete model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var chat = await _context.Chats
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync(c => c.Id == model.ChatId && c.ChatMembers.Any(u => u.Id == userId));

            if (chat == null)
            {
                return Json(new { success = false, message = "Чат не найден или доступ запрещён." });
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"Chat_{model.ChatId}").SendAsync("ChatDeleted", model.ChatId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToChat([FromBody] UserAdd model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var chat = await _context.Chats
                .Include(c => c.ChatMembers)
                .FirstOrDefaultAsync(c => c.Id == model.ChatId && c.ChatMembers.Any(u => u.Id == userId));

            if (chat == null)
            {
                return Json(new { success = false, message = "Чат не найден или доступ запрещён." });
            }

            var userToAdd = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Username);
            if (userToAdd == null)
            {
                return Json(new { success = false, message = "Пользователь не найден." });
            }

            if (chat.ChatMembers.Any(u => u.Id == userToAdd.Id))
            {
                return Json(new { success = false, message = "Пользователь уже в чате." });
            }

            chat.ChatMembers.Add(userToAdd);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"Chat_{model.ChatId}").SendAsync("UserAdded", model.Username);
            await _hubContext.Clients.User(userToAdd.Id.ToString()).SendAsync("ChatCreated", model.ChatId, chat.Title);
            return Json(new { success = true, username = model.Username });
        }
    }

    public class ChatEdit
    {
        public int ChatId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ChatDelete
    {
        public int ChatId { get; set; }
    }

    public class UserAdd
    {
        public int ChatId { get; set; }
        public string Username { get; set; }
    }
}