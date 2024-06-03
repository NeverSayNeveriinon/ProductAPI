using Core.DTO.Product;


namespace Core.ServiceContracts;

/// <summary>
/// Represents business logic for managing Product entity
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Add a product object to products list
    /// </summary>
    /// <param name="productAddRequest">Product object to be added</param>
    /// <return>Returns the new product object after adding it</return>
    public Task<ProductResponse> AddProduct(ProductRequest? productAddRequest);


    /// <summary>
    /// Retrieve all Product objects from products list
    /// </summary>
    /// <returns>Returns all existing Products</returns>
    public Task<List<ProductResponse>> GetAllProducts(string? FilterEmail);


    /// <summary>
    /// Retrieve a Product object from products list based on given key
    /// </summary>
    /// <param name="productKey">the product key (Data and Email) to be searched for</param>
    /// <returns></returns>
    public Task<ProductResponse?> GetProductByKey(ProductKey? productKey);
    

    /// <summary>
    /// Find the object in products list and update it with new details, then returns the 'Product' object
    /// </summary>
    /// <param name="productUpdateRequest">the new product object to be updated</param>
    /// <param name="productKey">the product key (Data and Email) to be searched for</param>
    /// <returns>Returns the product with updated details</returns>
    public Task<ProductResponse?> UpdateProduct(ProductRequest? productUpdateRequest, ProductKey? productKey);


    /// <summary>
    /// Find and Delete the product object with given 'key' from the 'products list'
    /// </summary>
    /// <param name="productKey">the product key (Data and Email) to be searched for</param>
    /// <returns>Returns 'True' if product is deleted and if it isn't 'False'</returns>
    public Task<bool?> DeleteProduct(ProductKey? productKey); 
}