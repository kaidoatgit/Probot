using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Probot.Data;
public static class DependencyInjection
{
    public static IServiceCollection AddProbotContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProbotContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("ProbotDatabase")
                , b => b.MigrationsAssembly("Probot.Data")
                );
            // .LogTo(Console.WriteLine, LogLevel.Information); // Logs SQL queries to console;
        });

        return services;
    }

    #region Database
    public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ProbotContext>();
        context.Database.Migrate();
        await context.EnsureSeedData();        
    }

    private static async Task EnsureSeedData(this ProbotContext context)
    {

        context.Database.EnsureCreated();

        /***
            * Example of removing the free product from the existing options
            * I can manipulate data here and the migrations could be responsible only for adding/removing fields
            * 
            * var freeProduct = context.Products.Where(p => p.ProductName == ProductName.Free).ToList();
            * if (freeProduct.Any())
            * {
            *    context.Products.RemoveRange(freeProduct);
            * }
            * await context.SaveChangesAsync();
            *  
        **/

        await Task.CompletedTask;
    }
    #endregion
}
