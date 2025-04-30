using Note2Book.Models;

namespace Note2Book.Interfaces;

public interface INotificationClient
{
    public Task SendMessageAsync(Message message);

    public Task ReceiveMessage(string message);
    
    public Task ReceiveNotification(string message);
    
    public Task ReceiveNotifications(string message);
    
    public Task ReceiveNotificationCount(int count);
}