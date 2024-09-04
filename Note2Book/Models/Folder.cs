namespace Note2Book.Models;

public class Folder : BaseModel
{
    public string Text { get; set; }
    
    public List<Note> Notes { get; set; }
    
    public User User { get; set; }
}