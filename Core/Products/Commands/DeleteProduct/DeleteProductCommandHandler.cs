using Core.Domain;
using Core.Domain.RepositoryContracts;
using MediatR;

namespace Core.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool?>
{
    private readonly IProductRepository _productRepository;
    
    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    
    public async Task<bool?> Handle(DeleteProductCommand? request, CancellationToken cancellationToken)
    {
        // if 'Key' is null
        ArgumentNullException.ThrowIfNull(request?.ProductKey,"The 'Product Key' parameter is Null");

        Product? product = await _productRepository.GetProductByKey(request.ProductKey);
        
        // if 'Key' is invalid (doesn't exist)
        if (product == null) 
        {
            return null;
        }
    
        bool result = await _productRepository.DeleteProduct(product);
            
        return result;
    }
}