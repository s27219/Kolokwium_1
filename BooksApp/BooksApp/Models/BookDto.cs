namespace BooksApp.Models;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<AuthorsDTO> Authors { get; set; } = null!;
}

public class AuthorsDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}