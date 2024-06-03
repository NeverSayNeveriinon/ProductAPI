using Microsoft.EntityFrameworkCore;


namespace API.Helpers;

public static class WebApplicationExtensions
{
    public static void CreateDatabase<T>(this WebApplication app) where T : DbContext
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<T>();
            context?.Database.EnsureDeleted();
            context?.Database.EnsureCreated();
            // context?.Database.Migrate();
        }
    }
}
