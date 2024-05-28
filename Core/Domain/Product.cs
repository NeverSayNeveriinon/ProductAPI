using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class Product
{
    public DateTime ProduceDate { get; set; }
    
    [EmailAddress]
    [StringLength(60)]
    public string? ManufactureEmail { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; }
    
    [Phone]
    public string? ManufacturePhone { get; set; }
    
    public bool IsAvailable { get; set; }
}