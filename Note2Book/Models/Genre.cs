using System.Collections;

namespace Note2Book.Models;

public class Genre : BaseModel
{
    public string Title { get; set; }

    public List<Book> Books { get; set; } = [];
}