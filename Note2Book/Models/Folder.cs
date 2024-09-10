namespace Note2Book.Models;

public class Folder : BaseModel
{
    public string Text { get; set; }

    public string? ImageUrl { get; set; }
    
    public List<Note> Notes { get; set; }
    
    public User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}