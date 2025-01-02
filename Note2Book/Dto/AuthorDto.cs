using Note2Book.Models;

namespace Note2Book.Dto;

public class AuthorDto : BaseModel
{
    public string Name { get; set; }
    
    public string LastName { get; set; }
}