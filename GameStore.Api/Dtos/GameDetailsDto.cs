namespace GameStore.Api.Dtos;

public record class GameDetailsDto(
    int Id,
    string Name, 
    int GenreID, 
    decimal Price,
    DateOnly ReleaseDate
);
