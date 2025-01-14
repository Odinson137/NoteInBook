namespace Note2Book.Models;

public class Activity : BaseModel
{
    public User User { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}