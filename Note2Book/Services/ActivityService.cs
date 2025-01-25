using Note2Book.Data;
using Note2Book.Interfaces;
using Note2Book.Models;

namespace Note2Book.Services;

public class ActivityService : IActivityService
{
    private readonly DataContext _context;

    public ActivityService(DataContext context)
    {
        _context = context;
    }

    public async Task AddActivityAsync(Activity activity)
    {
        await _context.Activities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }

    public async Task AddActivityAsync(User user)
    {
        var activity = new Activity { User = user };
        await AddActivityAsync(activity);
    }

    public async Task AddActivityAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new NullReferenceException("User not found");

        var activity = new Activity { User = user };
        await AddActivityAsync(activity);
    }

    public void AddActivity(Activity activity)
    {
        _context.Activities.Add(activity);
        _context.SaveChanges();
    }

    public void AddActivity(User user)
    {
        var activity = new Activity { User = user };
        AddActivity(activity);
    }

    public void AddActivity(int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null) throw new NullReferenceException("User not found");

        var activity = new Activity { User = user };
        AddActivity(activity);
    }
    
    public ICollection<Activity> GetDayActivities(int userId)
    {
        var activities = _context.Activities.Where(c => c.User.Id == userId).ToList();
        return activities;
    }
}