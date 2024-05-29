using System.Linq.Expressions;
using Core.Domain;
using Core.Domain.RepositoryContracts;
using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    
    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
   

    public async Task<List<Product>> GetAllProducts()
    {
        var movies = _dbContext.Products.AsNoTracking();
        
        List<Product> moviesList = await movies.ToListAsync();
        
        return moviesList;
    }

    public async Task<Product?> GetProductByKey(DateTime ProduceDate, string ManufactureEmail)
    {
        Product? movie = await _dbContext.Products.AsNoTracking()
                                                  .FirstOrDefaultAsync(movieItem => movieItem.ProduceDate == ProduceDate 
                                                                                    && movieItem.ManufactureEmail == ManufactureEmail);
        return movie;
    }

     
    public async Task<Product> AddProduct(Product movie)
    {
        _dbContext.Products.Add(movie);
        await _dbContext.SaveChangesAsync();

        return movie;
    }
    
    public async Task<Product> UpdateProduct(Product movie, Product updatedProduct)
    {
        // _dbContext.Attach(movie);
        _dbContext.Entry(movie).State = EntityState.Modified;

        _dbContext.Entry(movie).CurrentValues.SetValues(updatedProduct);
        
        await _dbContext.SaveChangesAsync();
        return movie;
    }
    
    public async Task<bool> DeleteProduct(Product movie)
    {
        _dbContext.Products.Remove(movie);
        int rowsAffected = await _dbContext.SaveChangesAsync();
        
        bool result = rowsAffected > 0 ? true : false;
        return result;
    }
    

    public async Task<List<Product>> GetFilteredProducts(Expression<Func<Product, bool>> predicate)
    {
        List<Product> moviesList = await _dbContext.Products.Where(predicate)
                                                            .ToListAsync();
        return moviesList;
    }
}