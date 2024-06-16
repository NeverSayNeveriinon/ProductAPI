using System.ComponentModel.DataAnnotations;

namespace Core.Products.Common_DTOs;

public class ProductKey
{
    public DateTime ProduceDate { get; set; }
    
    [EmailAddress(ErrorMessage = "The 'Email Address' Should Be in a Proper Format!!!")]
    public string ManufactureEmail { get; set; }
}