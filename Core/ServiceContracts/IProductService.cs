using Core.DTO;

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
    public Task<List<ProductResponse>> GetAllProducts();


    /// <summary>
    /// Retrieve a Product object from products list based on given id
    /// </summary>
    /// <param name="ID">the product id to be searched for</param>
    /// <returns></returns>
    public Task<ProductResponse?> GetProductByID(Guid? ID);
    

    /// <summary>
    /// Find the object in products list and update it with new details, then returns the 'Product' object
    /// </summary>
    /// <param name="productUpdateRequest"></param>
    /// <param name="productID"></param>
    /// <returns>Returns the product with updated details</returns>
    public Task<ProductResponse?> UpdateProduct(ProductRequest? productUpdateRequest, Guid? productID);


    /// <summary>
    /// Find and Delete the product object with given 'id' from the 'products list'
    /// </summary>
    /// <param name="ID"></param>
    /// <returns>Returns 'True' if product is deleted and if it isn't 'False'</returns>
    public Task<bool?> DeleteProduct(Guid? ID); 
}