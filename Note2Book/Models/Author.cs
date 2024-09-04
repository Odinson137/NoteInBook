namespace Note2Book.Models;

public class Author : BaseModel
{
    public string Name { get; set; }
    
    public string LastName { get; set; }
    
    public List<Book> Books { get; set; } = [];
}