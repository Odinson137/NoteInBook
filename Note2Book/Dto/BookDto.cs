namespace Note2Book.Dto;

public class BookDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public List<GenreDto> Genres { get; set; } = [];
    
    public DateTime CreateAt { get; set; } = DateTime.Now;

    public AuthorDto? Author { get; set; }
}