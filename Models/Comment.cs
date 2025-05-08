using Proje.Models;

public class Comment
{
    public int Id { get; set; }
    public int? BookId { get; set; }
    public int? MovieId { get; set; }
    public string Username { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Book? Book { get; set; }
    public Movie? Movie { get; set; }
}
