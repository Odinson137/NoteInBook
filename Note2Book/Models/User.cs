namespace Note2Book.Models;

public class User : BaseModel
{
    public string Name { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public string? ProfileImage { get; set; }
    
    public List<Book> Books { get; set; }
}