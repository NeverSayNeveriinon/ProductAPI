using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class Product
{
    public DateTime ProduceDate { get; set; }
    
    [StringLength(60)]
    // [EmailAddress]
    public string? ManufactureEmail { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(11)]
    // [RegularExpression(@"^0[0-9]{10}$")] // exp: 09001234567
    public string? ManufacturePhone { get; set; }
    
    public bool IsAvailable { get; set; }
}