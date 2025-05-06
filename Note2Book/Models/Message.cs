using System.ComponentModel.DataAnnotations;

namespace Note2Book.Models;

public class ForumMessage : BaseModel
{
    [Required] public string Text { get; set; }

    public int ForumId { get; set; }
    public Forum Forum { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}