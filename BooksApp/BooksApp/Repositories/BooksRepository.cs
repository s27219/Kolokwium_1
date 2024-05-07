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
	    
    /*public class BookDto
    {
	    public int Id { get; set; }
	    public string Title { get; set; } = string.Empty;
	    public List<AuthorDTO> Authors { get; set; } = null!;
    }

    public class AuthorDTO
    {
	    public string FirstName { get; set; } = string.Empty;
	    public string LastName { get; set; } = string.Empty;
    }*/

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
    
    /*public async Task<AnimalDto> GetAnimal(int id)
    {
	    var query = @"
        SELECT 
            Animal.ID AS AnimalID,
            Animal.Name AS AnimalName,
            Type,
            AdmissionDate,
            Owner.ID as OwnerID,
            FirstName,
            LastName,
            Procedure_Animal.Date,
            [Procedure].Name AS ProcedureName,
            [Procedure].Description
        FROM Animal
        JOIN Owner ON Owner.ID = Animal.Owner_ID
        LEFT JOIN Procedure_Animal ON Procedure_Animal.Animal_ID = Animal.ID
        LEFT JOIN [Procedure] ON [Procedure].ID = Procedure_Animal.Procedure_ID
        WHERE Animal.ID = @ID";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand(query, connection);
	    command.Parameters.AddWithValue("@ID", id);

	    await connection.OpenAsync();
	    var reader = await command.ExecuteReaderAsync();

	    AnimalDto animalDto = null;

	    while (await reader.ReadAsync())
	    {
		    if (animalDto == null)
		    {
			    animalDto = new AnimalDto
			    {
				    Id = reader.GetInt32(reader.GetOrdinal("AnimalID")),
				    Name = reader.GetString(reader.GetOrdinal("AnimalName")),
				    Type = reader.GetString(reader.GetOrdinal("Type")),
				    AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
				    Owner = new OwnerDto
				    {
					    Id = reader.GetInt32(reader.GetOrdinal("OwnerID")),
					    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
					    LastName = reader.GetString(reader.GetOrdinal("LastName"))
				    },
				    Procedures = new List<ProcedureDto>()
			    };
		    }

		    if (!reader.IsDBNull(reader.GetOrdinal("ProcedureName")))
		    {
			    animalDto.Procedures.Add(new ProcedureDto
			    {
				    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
				    Name = reader.GetString(reader.GetOrdinal("ProcedureName")),
				    Description = reader.GetString(reader.GetOrdinal("Description"))
			    });
		    }
	    }

	    if (animalDto == null)
		    throw new KeyNotFoundException($"No animal found with ID {id}.");

	    return animalDto;
    }*/
}