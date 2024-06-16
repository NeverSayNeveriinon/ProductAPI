﻿using System.ComponentModel.DataAnnotations;
using Core.Products.Common_DTOs;
using MediatR;

namespace Core.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductResponse>
{
    [Required(ErrorMessage = "The 'Product Name' Can't Be Blank!!!")]
    [StringLength(50, ErrorMessage = "The 'Product Name' Can't Be More Than 50 Characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "The 'Availability of Product' should be specified!!!")]
    public bool? IsAvailable { get; set; }
}