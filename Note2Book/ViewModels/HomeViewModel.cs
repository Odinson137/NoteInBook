using Note2Book.Models;

namespace Note2Book.ViewModels;

public class HomeViewModel
{
    public ICollection<BookViewModel> FavoriteBooks { get; set; } = [];

    public ICollection<Favorite> BookStatus { get; set; } = [];

    public ICollection<Activity> Activities { get; set; } = [];
}