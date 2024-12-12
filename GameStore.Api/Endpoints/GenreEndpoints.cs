using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenreEndpoints
{
    public static RouteGroupBuilder MapGenresEnpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres");

        group.MapGet("/", async(GameStoreContext dbcontext) => 
            await dbcontext.Genres
                        .Select(genre => genre.ToDto())
                        .AsNoTracking()
                        .ToListAsync()
        );
        
        return group;
    }
}
