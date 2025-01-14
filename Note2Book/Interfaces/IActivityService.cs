using Note2Book.Models;

namespace Note2Book.Interfaces;

public interface IActivityService
{
    public Task AddActivityAsync(Activity activity);
    public Task AddActivityAsync(User user);
    public Task AddActivityAsync(int userId);
    public void AddActivity(Activity activity);

    public void AddActivity(User user);

    public void AddActivity(int userId);

    ICollection<Activity> GetDayActivities(int userId);
}