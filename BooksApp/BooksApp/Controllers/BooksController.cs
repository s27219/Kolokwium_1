using BooksApp.Models;
using BooksApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BooksApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    
    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetBooksAuthors(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
        {
            return NotFound($"Book with given ID - {id} doesn't exist");
        }

        var bookDto = await _booksRepository.GetBooksAuthors(id);
        return Ok(bookDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddBooks(BookDto bookDto)
    {

        await _booksRepository.AddBookWithAuthors(bookDto);

        return Created(Request.Path.Value ?? "api/books", bookDto);
    }
}