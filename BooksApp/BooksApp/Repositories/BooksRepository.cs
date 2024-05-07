using BooksApp.Models;
using Microsoft.Data.SqlClient;

namespace BooksApp.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;
    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BookDto> GetBooksAuthors(int id)
    {
	    var query = @"SELECT books.PK AS Id, books.title AS Title, authors.first_name as FirstName, authors.last_name as LastName FROM books
    	LEFT JOIN books_authors ON books_authors.FK_book = books.PK
        LEFT JOIN authors ON authors.PK = books_authors.FK_author
        WHERE books.PK = @Id;";
        
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand(query, connection);
	    command.Parameters.AddWithValue("@Id", id);

	    await connection.OpenAsync();
	    var reader = await command.ExecuteReaderAsync();

	    BookDto bookDto = null;
	    

	    while (await reader.ReadAsync())
	    {
		    if (bookDto == null)
		    {
			    bookDto = new BookDto
			    {
				    Id = reader.GetInt32(reader.GetOrdinal("Id")),
				    Title = reader.GetString(reader.GetOrdinal("Title")),
				    Authors = new List<AuthorsDTO>()
			    };
		    }

		    if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
		    {
			    bookDto.Authors.Add(new AuthorsDTO
			    {
				    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
				    LastName = reader.GetString(reader.GetOrdinal("LastName"))
			    });
		    }
	    }

	    if (bookDto == null)
		    throw new KeyNotFoundException($"No Book found with ID {id}.");

	    return bookDto;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
	    var query = "SELECT 1 FROM books WHERE PK = @ID";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);

	    await connection.OpenAsync();

	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }

    public async Task AddBookWithAuthors(BookDto bookDto)
    {
	    var insert = @"INSERT INTO books VALUES(@Title);
					   SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@Title", bookDto.Title);

	    await connection.OpenAsync();
	    
	    var transaction = await connection.BeginTransactionAsync();
	    command.Transaction = transaction as SqlTransaction;
	    
	    try
	    {
		    var id = await command.ExecuteScalarAsync();
    
		    foreach (var procedure in bookDto.Authors)
		    {
			    command.Parameters.Clear();
			    command.CommandText = "INSERT INTO authors VALUES(@FirstName, @LastName) SELECT @@IDENTITY AS ID";
			    command.Parameters.AddWithValue("@FirstName", procedure.FirstName);
			    command.Parameters.AddWithValue("@LastName", procedure.LastName);

			    var bookId = await command.ExecuteScalarAsync();
			    
			    command.Parameters.Clear();
			    command.CommandText = "INSERT INTO books_authors VALUES(@id, @LastName)";
			    command.Parameters.AddWithValue("@id", id);
			    command.Parameters.AddWithValue("@bookId", bookId);

			    await command.ExecuteNonQueryAsync();
		    }

		    await transaction.CommitAsync();
	    }
	    catch (Exception)
	    {
		    await transaction.RollbackAsync();
		    throw;
	    }
	    
	    throw new NotImplementedException();
    }
}