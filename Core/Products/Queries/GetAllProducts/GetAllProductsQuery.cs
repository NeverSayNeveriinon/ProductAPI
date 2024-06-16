using System.ComponentModel.DataAnnotations;
using MediatR;


namespace Core.DTO.Product;

public class GetAllProductsQuery : IRequest<List<ProductResponse>>
{
    [EmailAddress(ErrorMessage = "The 'Email Address' Should Be in a Proper Format!!!")]
    public string? FilterByEmail;
}