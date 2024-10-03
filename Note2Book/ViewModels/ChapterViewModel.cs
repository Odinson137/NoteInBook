using Note2Book.Models;

namespace Note2Book.ViewModels
{
    public class ChapterViewModel
    {
        public Chapter Chapter { get; set; }
        public string PageText { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? NextChapter { get; set; }
        public int? PreviousChapter { get; set; }
    }
}