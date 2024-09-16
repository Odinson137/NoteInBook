using Note2Book.Models;

namespace Note2Book.ViewModels
{
    public class ChapterViewModel
    {
        public Chapter Chapter { get; set; }
        public string PageText { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public Chapter? NextChapter { get; set; }
        public Chapter? PreviousChapter { get; set; }
    }
}