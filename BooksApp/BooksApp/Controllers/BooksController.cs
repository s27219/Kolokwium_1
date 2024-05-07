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

        var bookDto = await _booksRepository.GetBooksAuthors(id);
        return Ok(bookDto);
    }
}