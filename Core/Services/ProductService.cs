using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO;
using Core.Helpers;
using Core.ServiceContracts;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    // private readonly HttpContext _httpContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductService(IProductRepository productRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        // _httpContext = httpContextAccessor.HttpContext!;
    }


    public async Task<ProductResponse> AddProduct(ProductRequest? productAddRequest)
    {
        // 'productAddRequest' is Null //
        ArgumentNullException.ThrowIfNull(productAddRequest,"The 'ProductRequest' object parameter is Null");


        // 'productAddRequest.Name' is Empty or Null//
        ArgumentNullException.ThrowIfNullOrEmpty(productAddRequest.Name,"The 'Product Name' in 'ProductRequest' object can't be blank");

        
        // Other Validations
        ValidationHelper.ModelValidation(productAddRequest);

        // 'productAddRequest.Name' is Duplicate //
        if ( (await _productRepository.GetFilteredProducts(product => product.Name == productAddRequest.Name))?.Count > 0)
        {
            throw new ArgumentException("The 'Product Name' is already exists");
        }

        
        // Converting from 'ProductAddRequest' to 'Product'
        Product product = productAddRequest.Adapt<Product>();
        product.ProduceDate = DateTime.Now;
        product.ManufactureEmail = _httpContextAccessor.HttpContext!.User.Identity!.Name!;
        product.ManufacturePhone =  _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result!.PhoneNumber;
        
        // adding the object to db
        await _productRepository.AddProduct(product);

        // Converting from 'Product' to 'ProductResponse'
        ProductResponse productResponse = product.Adapt<ProductResponse>();
        return productResponse;
    }

    public async Task<List<ProductResponse>> GetAllProducts()
    {
        List<Product> products = await _productRepository.GetAllProducts();
        List<ProductResponse> productsResponses = products.Select(prouduct => prouduct.Adapt<ProductResponse>()).ToList();
        return productsResponses;
    }

    public async Task<ProductResponse?> GetProductByKey(ProductKey? productKey)
    {
        // if 'productKey' is null
        ArgumentNullException.ThrowIfNull(productKey,"The 'ProductKey' parameter is Null");

        Product? product = await _productRepository.GetProductByKey(productKey);

        // if no product with corresponding 'Key' exist in 'products list' 
        if (product == null)
        {
            return null;
        }

        // if there is no problem
        ProductResponse productResponse = product.Adapt<ProductResponse>();

        return productResponse;  
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