using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.DTO.Product;
using Core.Helpers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Products.Commands.CreateProduct;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery,List<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    
    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    
    public async Task<List<ProductResponse>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        List<Product> products = new List<Product>();
        if (!string.IsNullOrEmpty(query.FilterByEmail))
        {
            products = await _productRepository.GetFilteredProducts(record => record.ManufactureEmail == query.FilterByEmail);
        }
        else
        {
            products = await _productRepository.GetAllProducts();
        }

        List<ProductResponse> productsResponses = products.Select(prouduct => prouduct.Adapt<ProductResponse>()).ToList();
        return productsResponses;
    }
}