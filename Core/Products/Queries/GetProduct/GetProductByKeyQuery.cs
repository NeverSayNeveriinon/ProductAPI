using System.ComponentModel.DataAnnotations;
using Core.Products.Common_DTOs;
using MediatR;

namespace Core.Products.Queries.GetProduct;

public class GetProductByKeyQuery : IRequest<ProductResponse?>
{
    [Required(ErrorMessage = "The 'Product Key' Can't Be Blank!!!")]
    public ProductKey? ProductKey;
}