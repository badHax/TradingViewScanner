using Microsoft.EntityFrameworkCore;
using TVScanner.Data;

namespace TVScanner.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<TVScannerContext>())
                {
                    context.Seed();
                }
            }
        }
    }
}
