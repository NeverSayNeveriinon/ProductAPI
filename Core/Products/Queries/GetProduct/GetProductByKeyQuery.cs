using System.ComponentModel.DataAnnotations;
using MediatR;


namespace Core.DTO.Product;

public class GetProductByKeyQuery : IRequest<ProductResponse?>
{
    [Required(ErrorMessage = "The 'Product Key' Can't Be Blank!!!")]
    public ProductKey? ProductKey;
}