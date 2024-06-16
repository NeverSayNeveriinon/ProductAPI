using System.ComponentModel.DataAnnotations;
using Core.Products.Common_DTOs;
using MediatR;

namespace Core.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<List<ProductResponse>>
{
    [EmailAddress(ErrorMessage = "The 'Email Address' Should Be in a Proper Format!!!")]
    public string? FilterByEmail;
}