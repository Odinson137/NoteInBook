using Note2Book.Models;

namespace Note2Book.ViewModels;

public class ConspectusViewModel
{
    public ICollection<Folder>? Folders { get; set; }
    
    public List<IGrouping<ShortBookViewModel, ShortCitationViewModel>>? Books { get; set; }
}