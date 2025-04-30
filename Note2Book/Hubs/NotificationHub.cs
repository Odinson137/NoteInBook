using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Note2Book.Data;
using Note2Book.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Note2Book.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly DataContext _context;

        public ChatHub(DataContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int chatId, string messageText)
        {
            var userId = int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            var chat = await _context.Chats.FindAsync(chatId);

            if (user == null || chat == null)
            {
                return;
            }

            var message = new Message
            {
                Text = messageText,
                User = user,
                Chat = chat
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.Group($"Chat_{chatId}").SendAsync("ReceiveMessage", user.Name, messageText, message.CreatedAt);
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
        }

        public override async Task OnConnectedAsync()
        {
            // Optionally, you can join default chats or perform initialization
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Clean up group memberships if needed
            await base.OnDisconnectedAsync(exception);
        }
    }
}