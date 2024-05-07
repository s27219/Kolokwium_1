using BooksApp.Models;

namespace BooksApp.Repositories;

public interface IBooksRepository
{
    Task<BookDto> GetBooksAuthors(int id);
}