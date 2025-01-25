using Note2Book.Models;

namespace Note2Book.ViewModels;

public class UserViewModel : BaseModel
{
    public string Name { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public List<BookViewModel> Books { get; set; }
}