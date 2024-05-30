using Core.Domain.RepositoryContracts;
using Core.ServiceContracts;
using Core.Services;
using Infrastructure.DatabaseContext;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        
        // Services IOC
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddTransient<IJwtService, JwtService>();
        
        
        // Database IOC
        var DBconnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>
        (options =>
        {
            options.UseSqlServer(DBconnectionString);
        });
        
        
        var app = builder.Build();

        // Middlewares //
        
        // Https
        app.UseHsts();
        app.UseHttpsRedirection();
        
        app.UseStaticFiles();
        app.MapControllers();

        
        app.Run();
    }
}