using Note2Book.Models;

namespace Note2Book.ViewModels;

public class BookViewModel : BaseViewModel
{
    public string Title { get; set; }
    
    public string Description { get; set; }

    public List<Genre> Genres { get; set; } = [];
    
    public DateTime CreateAt { get; set; } = DateTime.Now;

    public string? Image { get; set; }

    public List<Chapter> Chapters { get; set; } = [];
    
    public List<BookComment> Comments { get; set; } = [];
    public Author Author { get; set; }
    
    public bool IsLiked { get; set; }
    
    public List<Storage> Storages { get; set; } = [];
}