using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class ProductRequest
{
    [Required(ErrorMessage = "The 'Product Date and Time' Can't Be Blank!!!")]
    public DateTime? ProduceDate { get; set; }
    
    [Required(ErrorMessage = "The 'Email' Can't Be Blank!!!")]
    [StringLength(60, ErrorMessage = "The 'Email' Can't Be More Than 60 Characters")]
    [EmailAddress(ErrorMessage = "The 'Email' is not in a Correct Format")]
    public string ManufactureEmail { get; set; }
    
    [Required(ErrorMessage = "The 'Product Name' Can't Be Blank!!!")]
    [StringLength(50, ErrorMessage = "The 'Product Name' Can't Be More Than 50 Characters")]
    public string Name { get; set; }
    
    [StringLength(maximumLength:11, MinimumLength = 11, ErrorMessage = "The 'Phone Number' must be 11 Characters")]
    [RegularExpression("^0[0-9]{10}$", ErrorMessage = "The Number Should be '0XXXXXXXXXX'")] // exp: 09001234567
    public string? ManufacturePhone { get; set; }
    
    [Required(ErrorMessage = "The 'Availability of Product' should be specified!!!")]
    public bool? IsAvailable { get; set; }
}