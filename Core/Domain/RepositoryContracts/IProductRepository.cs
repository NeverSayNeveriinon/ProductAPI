using System.Linq.Expressions;

using Core.DTO.Product;


namespace Core.Domain.RepositoryContracts;

/// <summary>
/// Represents data access logic for managing Product entity
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Returns all products in the data store
    /// </summary>
    /// <returns>List of product objects from table</returns>
    Task<List<Product>> GetAllProducts();


    /// <summary>
    /// Returns a product object based on the given product Date & Email
    /// </summary>
    /// <param name="productKey">Produce Date & Manufacture Email as Key of Product to search</param>
    /// <returns>A product object or null</returns>
    Task<Product?> GetProductByKey(ProductKey productKey);


    /// <summary>
    /// Adds a product object to the data store
    /// </summary>
    /// <param name="product">Product object to add</param>
    /// <returns>Returns the product object after adding it to the table</returns>
    Task<Product> AddProduct(Product product);

    
    /// <summary>
    /// Deletes a product object based on the given product object
    /// </summary>
    /// <param name="product">Product object to delete</param>
    /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
    Task<bool> DeleteProduct(Product product);


    /// <summary>
    /// Updates a product object (product name and other details) based on the given product object (updatedProduct)
    /// </summary>
    /// <param name="product">Product object to be updated</param>
    /// <param name="updatedProduct">The updated Product object to apply to actual product object</param>
    /// <returns>Returns the updated product object</returns>
    Task<Product> UpdateProduct(Product product, Product updatedProduct);
    
    
    /// <summary>
    /// Returns all product objects based on the given expression
    /// </summary>
    /// <param name="predicate">LINQ expression to check</param>
    /// <returns>All matching products with given condition</returns>
    Task<List<Product>> GetFilteredProducts(Expression<Func<Product, bool>> predicate);
}