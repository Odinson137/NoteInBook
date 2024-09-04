namespace Note2Book.Models;

public class Note : BaseModel
{
    public string Text { get; set; }
    
    public User Author { get; set; }
    
    public DateTime DateTime { get; set; } = DateTime.Now;
}