using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;

namespace ProPayments.Service.Extensions
{
    public static class DataBaseExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<SubscriptionContext>();
            context.Database.Migrate();
            await context.EnsureSeedData();
        }
    }
}
