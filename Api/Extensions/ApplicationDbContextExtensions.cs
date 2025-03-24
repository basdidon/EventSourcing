using Microsoft.EntityFrameworkCore;

namespace Api.Extensions
{
    public static class ApplicationDbContextExtensions
    {
        public static void EnsureDatabaseCreated<T>(this IApplicationBuilder app, bool resetOnStart = false) where T : DbContext
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();

            if (context.Database.CanConnect())
            {
                if (resetOnStart)
                    context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            else
            {
                throw new Exception("can't connect to database");
            }
        }
    }
}
