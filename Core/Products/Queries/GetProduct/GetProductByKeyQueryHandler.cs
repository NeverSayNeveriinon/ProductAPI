using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO.Product;
using Core.Helpers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Products.Commands.CreateProduct;

public class GetProductByKeyQueryHandler : IRequestHandler<GetProductByKeyQuery,ProductResponse?>
{
    private readonly IProductRepository _productRepository;
    
    public GetProductByKeyQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    
    public async Task<ProductResponse?> Handle(GetProductByKeyQuery? query, CancellationToken cancellationToken)
    {
        // if 'productKey' is null
        ArgumentNullException.ThrowIfNull(query?.ProductKey,"The 'ProductKey' parameter is Null");

        Product? product = await _productRepository.GetProductByKey(query.ProductKey);

        // if no product with corresponding 'Key' exist in 'products list' 
        if (product == null)
        {
            return null;
        }

        // if there is no problem
        ProductResponse productResponse = product.Adapt<ProductResponse>();

        return productResponse;
    }
}