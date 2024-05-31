using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO;
using Core.Helpers;
using Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductService(IProductRepository productRepository, UserManager<ApplicationUser> userManager)
    {
        _productRepository = productRepository;
        _userManager = userManager;
    }


    public Task<ProductResponse> AddProduct(ProductRequest? productAddRequest)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductResponse>> GetAllProducts()
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponse?> GetProductByKey(ProductKey? productKey)
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponse?> UpdateProduct(ProductRequest? productUpdateRequest, ProductKey? productKey)
    {
        throw new NotImplementedException();
    }

    public Task<bool?> DeleteProduct(ProductKey? productKey)
    {
        throw new NotImplementedException();
    }
}