using Note2Book.Data;

namespace Note2Book.Models;

public class Favorite : BaseModel
{
    public Book Book { get; set; }
    
    public User User { get; set; }

    public UserBook UserBook { get; set; } = UserBook.Favorite;
}