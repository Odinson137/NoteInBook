using System.ComponentModel.DataAnnotations;

namespace Note2Book.Models;

public class Forum : BaseModel
{
    [Required] public int BookId { get; set; }
    public Book Book { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ForumMessage> Messages { get; set; } = [];
}