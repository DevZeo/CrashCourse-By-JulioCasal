using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        
        // GET /games
        group.MapGet("/", async(GameStoreContext dbContext) => await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync());

        // GET /games/1
        group.MapGet("/{id}", async(int id,  GameStoreContext dbContext) => 
        {
            Game? findGame = await dbContext.Games.FindAsync(id);

            return findGame is null ? Results.NotFound() : Results.Ok(findGame.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async(CreateGamesDto newGame, GameStoreContext dbContext) => 
        {
            Game game = newGame.ToEntity();
            
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();


            return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id}, game.ToGameDetailsDto());
        });

        // PUT /games
        group.MapPut("/{id}", async(int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if(existingGame is null)
            {
                // You can create a new resource from payload instead as well
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();


            return Results.NoContent();
        });


        // DELETE /games
        group.MapDelete("/{id}", async(int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}
