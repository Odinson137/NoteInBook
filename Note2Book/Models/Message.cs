using System.ComponentModel.DataAnnotations;

namespace Note2Book.Models;

public class Message : BaseModel
{
    [MaxLength(1000)] public string? Text { get; set; }

    [Required] public User? User { get; set; }

    [Required] public Chat Chat { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}