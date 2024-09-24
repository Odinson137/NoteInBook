namespace Note2Book.Models;

public class BookComment : BaseModel
{
    public User Author { get; set; }
    
    public string Text { get; set; }
    
    public Book Book { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}