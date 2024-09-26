using Note2Book.Data;

namespace Note2Book.Models;

public class Storage : BaseModel
{
    public Book Book { get; set; }

    public string Url { get; set; }
    
    public FileType FileType { get; set; }
}