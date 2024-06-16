using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.Products.Common_DTOs;
using Mapster;
using MediatR;

namespace Core.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse?>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    
    public async Task<ProductResponse?> Handle(UpdateProductCommand? request, CancellationToken cancellationToken)
    {
        // if 'product ID' is null
        ArgumentNullException.ThrowIfNull(request?.ProductKey,"The 'ProductKey' parameter is Null");
        
        // if 'request' is null
        ArgumentNullException.ThrowIfNull(request,"The 'ProductRequest' object parameter is Null");

        // 'request.Name' is Empty or Null//
        ArgumentException.ThrowIfNullOrEmpty(request.Name,"The 'Product Name' in 'ProductRequest' object can't be blank");

        
        Product? product = await _productRepository.GetProductByKey(request.ProductKey);
        
        // if 'key' is invalid (doesn't exist)
        if (product == null)
        {
            return null;
        }
            
        Product updatedProduct = await _productRepository.UpdateProduct(product, request.Adapt<Product>());

        return updatedProduct.Adapt<ProductResponse>();
    }
}