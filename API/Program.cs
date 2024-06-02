using System.Text;
using Core;
using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.ServiceContracts;
using Core.Services;
using Infrastructure.DatabaseContext;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        
        // Identity IOC
        builder.Services.AddIdentity<ApplicationUser,ApplicationRole>(options => 
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 3; //Eg: AB12AB
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddUserStore<UserStore<ApplicationUser,ApplicationRole,AppDbContext,Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole,AppDbContext,Guid>>()
            .AddDefaultTokenProviders();
        
        
        // JWT
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
        
        // Swagger
        // Generates description for all endpoints (action methods)
        builder.Services.AddEndpointsApiExplorer(); 
        // Generates OpenAPI specification
        builder.Services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments("wwwroot/ProductApp.xml"); // For Reading the 'XML' comments
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please Enter JWT Token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        }); 

        
        var app = builder.Build();
        
        
        // using (var scope = app.Services.CreateScope())
        // {
        //     var context = scope.ServiceProvider.GetService<AppDbContext>();
        //     context?.Database.EnsureDeleted();
        //     context?.Database.EnsureCreated();
        //     // context?.Database.Migrate();
        // }

        // Middlewares //
        
        // Https
        app.UseHsts();
        app.UseHttpsRedirection();
        
        // Swagger
        app.UseSwagger(); // Creates endpoints for swagger.json
        app.UseSwaggerUI(); // Creates swagger UI for testing all endpoints (action methods)
        
        app.UseStaticFiles();

        app.UseRouting(); 
        app.UseAuthentication(); 
        app.UseAuthorization(); 
        app.MapControllers(); 
        
        app.Run();
    }
}