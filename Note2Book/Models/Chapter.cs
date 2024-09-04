namespace Note2Book.Models;

public class Chapter : BaseModel
{
    public string Title { get; set; }
    
    public int Number { get; set; }
    
    public Book Book { get; set; }
    
    public string Text { get; set; }
}