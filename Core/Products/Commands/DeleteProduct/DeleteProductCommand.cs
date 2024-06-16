using System.ComponentModel.DataAnnotations;
using Core.DTO.Product;
using MediatR;

namespace Core.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<bool?>
{
    [Required(ErrorMessage = "The 'Product Key' Can't Be Blank!!!")]
    public ProductKey? ProductKey;
}