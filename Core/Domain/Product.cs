using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class Product
{
    [StringLength(50)]
    public string Name { get; set; }
    
    public DateTime ProduceDate { get; set; }
    
    [EmailAddress]
    public string? ManufactureEmail { get; set; }
    
    [Phone]
    public string? ManufacturePhone { get; set; }
    
    public bool IsAvailable { get; set; }
}