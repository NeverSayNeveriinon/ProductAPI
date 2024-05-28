using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
                    .HasKey(product => new { product.ManufactureEmail, product.ProduceDate });
    }
}