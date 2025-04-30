using System.ComponentModel.DataAnnotations;

namespace Note2Book.Models;

public class Chat : BaseModel
{
    [Required] [MaxLength(100)] public string Title { get; set; } = null!;

    [MaxLength(500)] public string? Description { get; set; }

    public ICollection<User> ChatMembers { get; set; } = [];

    public ICollection<Message> Messages { get; set; } = [];
}