using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.Products.Common_DTOs;
using Infrastructure.DatabaseContext;


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
        var products = _dbContext.Products.AsNoTracking();
        
        List<Product> productsList = await products.ToListAsync();
        
        return productsList;
    }

    public async Task<Product?> GetProductByKey(ProductKey productKey)
    {
        Product? product = await _dbContext.Products.AsNoTracking()
                                                  .FirstOrDefaultAsync(productItem => productItem.ProduceDate == productKey.ProduceDate 
                                                                                    && productItem.ManufactureEmail == productKey.ManufactureEmail);
        return product;
    }

     
    public async Task<Product> AddProduct(Product product)
    {
        var entity = _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        Product productEntity = entity.Entity;
        return productEntity;
    }
    
    public async Task<Product> UpdateProduct(Product product, Product updatedProduct)
    {
        _dbContext.Entry(product).State = EntityState.Modified;

        product.Name = updatedProduct.Name;
        product.IsAvailable = updatedProduct.IsAvailable;
        
        await _dbContext.SaveChangesAsync();
        return product;
    }
    
    public async Task<bool> DeleteProduct(Product product)
    {
        _dbContext.Products.Remove(product);
        int rowsAffected = await _dbContext.SaveChangesAsync();
        
        bool result = rowsAffected > 0 ? true : false;
        return result;
    }
    

    public async Task<List<Product>> GetFilteredProducts(Expression<Func<Product, bool>> predicate)
    {
        List<Product> productsList = await _dbContext.Products.Where(predicate)
                                                            .ToListAsync();
        return productsList;
    }
}