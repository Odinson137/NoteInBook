namespace Note2Book.Models;

public class Citation: BaseModel
{
    public string Text { get; set; }
    
    public User Author { get; set; }
    
    public DateTime DateTime { get; set; } = DateTime.Now;
    
    public Chapter Chapter { get; set;}
    
    public int Start { get; set; }
    
    public int End { get; set; }
}