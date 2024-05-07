using BooksApp.Models;

namespace BooksApp.Repositories;

public interface IBooksRepository
{
    Task<BookDto> GetBooksAuthors(int id);
    Task<bool> DoesBookExist(int id);
    Task AddBookWithAuthors(BookDto bookDto);
}