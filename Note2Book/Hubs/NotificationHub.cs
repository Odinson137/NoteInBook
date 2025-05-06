using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Note2Book.Data;
using Note2Book.Models;
using System;
using System.Threading.Tasks;

namespace Note2Book.Hubs
{
    [Authorize]
    public class ForumHub : Hub
    {
        private readonly DataContext _context;

        public ForumHub(DataContext context)
        {
            _context = context;
        }

        public async Task JoinForum(int forumId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Forum_{forumId}");
        }

        public async Task LeaveForum(int forumId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Forum_{forumId}");
        }

        public async Task SendMessage(int forumId, string message)
        {
            var userId = int.Parse(Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);
            var forum = await _context.Forums.FindAsync(forumId);

            if (user != null && forum != null)
            {
                var newMessage = new ForumMessage
                {
                    Text = message,
                    ForumId = forumId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ForumMessages.Add(newMessage);
                await _context.SaveChangesAsync();

                await Clients.Group($"Forum_{forumId}").SendAsync("ReceiveMessage", user.Name, message, newMessage.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}