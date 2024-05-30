using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO;
using Core.ServiceContracts;

namespace Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<ProductResponse> AddProduct(ProductRequest? productAddRequest)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductResponse>> GetAllProducts()
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponse?> GetProductByID(Guid? ID)
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponse?> UpdateProduct(ProductRequest? productUpdateRequest, Guid? productID)
    {
        throw new NotImplementedException();
    }

    public Task<bool?> DeleteProduct(Guid? ID)
    {
        throw new NotImplementedException();
    }
}