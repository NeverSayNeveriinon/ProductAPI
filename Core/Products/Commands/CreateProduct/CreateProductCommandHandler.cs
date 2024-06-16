using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO.Product;
using Core.Helpers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public CreateProductCommandHandler(IProductRepository productRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    
    public async Task<ProductResponse> Handle(CreateProductCommand? request, CancellationToken cancellationToken)
    {
        // 'request' is Null //
        ArgumentNullException.ThrowIfNull(request,"The 'ProductRequest' object parameter is Null");


        // 'request.Name' is Empty or Null//
        ArgumentException.ThrowIfNullOrEmpty(request.Name,"The 'Product Name' in 'ProductRequest' object can't be blank");

        
        // Other Validations
        ValidationHelper.ModelValidation(request);

        // 'request.Name' is Duplicate //
        if ( (await _productRepository.GetFilteredProducts(product => product.Name == request.Name))?.Count > 0)
        {
            throw new ArgumentException("The 'Product Name' is already exists");
        }

        
        // Converting from 'CreateProductCommand' to 'Product'
        Product product = request.Adapt<Product>();
        product.ProduceDate = DateTime.Now;
        product.ManufactureEmail = _httpContextAccessor.HttpContext!.User.Identity!.Name!;
        product.ManufacturePhone =  _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result!.PhoneNumber;
        
        // adding the object to db`
        await _productRepository.AddProduct(product);

        // Get the object added to db from repository and Convert from 'Product' to 'ProductResponse'
        var productFromRepository = await _productRepository.GetProductByKey(new ProductKey()
        {
            ProduceDate = product.ProduceDate,
            ManufactureEmail = product.ManufactureEmail
        });
        
        ProductResponse productResponse = productFromRepository.Adapt<ProductResponse>();
        return productResponse;
    }
}