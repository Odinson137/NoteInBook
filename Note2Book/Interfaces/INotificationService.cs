using Note2Book.Models;

namespace Note2Book.Interfaces;

public interface INotificationService
{
    public Task SendMessage(Message message, int toUserId);
}