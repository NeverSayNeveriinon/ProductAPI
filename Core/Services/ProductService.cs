using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO.Product;
using Core.Helpers;
using Core.ServiceContracts;


namespace Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductService(IProductRepository productRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<ProductResponse> AddProduct(ProductRequest? productAddRequest)
    {
        // 'productAddRequest' is Null //
        ArgumentNullException.ThrowIfNull(productAddRequest,"The 'ProductRequest' object parameter is Null");


        // 'productAddRequest.Name' is Empty or Null//
        ArgumentException.ThrowIfNullOrEmpty(productAddRequest.Name,"The 'Product Name' in 'ProductRequest' object can't be blank");

        
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

    public async Task<List<ProductResponse>> GetAllProducts(string? FilterEmail)
    {
        List<Product> products = new List<Product>();
        if (!string.IsNullOrEmpty(FilterEmail))
        {
            products = await _productRepository.GetFilteredProducts(record => record.ManufactureEmail == FilterEmail);
        }
        else
        {
            products = await _productRepository.GetAllProducts();
        }

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

    public async Task<ProductResponse?> UpdateProduct(ProductRequest? productUpdateRequest, ProductKey? productKey)
    {
        // if 'product ID' is null
        ArgumentNullException.ThrowIfNull(productKey,"The 'ProductKey' parameter is Null");
        
        // if 'productUpdateRequest' is null
        ArgumentNullException.ThrowIfNull(productUpdateRequest,"The 'ProductRequest' object parameter is Null");

        // 'productUpdateRequest.Name' is Empty or Null//
        ArgumentException.ThrowIfNullOrEmpty(productUpdateRequest.Name,"The 'Product Name' in 'ProductRequest' object can't be blank");

        
        Product? product = await _productRepository.GetProductByKey(productKey);
        
        // if 'ID' is invalid (doesn't exist)
        if (product == null)
        {
            return null;
        }
            
        Product updatedProduct = await _productRepository.UpdateProduct(product, productUpdateRequest.Adapt<Product>());

        return updatedProduct.Adapt<ProductResponse>();
    }

    public async Task<bool?> DeleteProduct(ProductKey? productKey)
    {
        // if 'Key' is null
        ArgumentNullException.ThrowIfNull(productKey,"The 'ProductKey' parameter is Null");

        Product? product = await _productRepository.GetProductByKey(productKey);
        
        // if 'Key' is invalid (doesn't exist)
        if (product == null) 
        {
            return null;
        }
    
        bool result = await _productRepository.DeleteProduct(product);
            
        return result;
    }
}