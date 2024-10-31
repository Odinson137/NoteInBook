using Note2Book.Models;

public class CitationViewModel
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Comment { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    
    public int ChapterId { get; set; }
    public DateTime DateTime { get; set; }
}