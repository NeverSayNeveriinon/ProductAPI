using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain;

// Key: {ProduceDate, ManufactureEmail}
public class Product
{
    public DateTime ProduceDate { get; set; }
    
    [StringLength(60)]
    [Unicode(false)]
    public string ManufactureEmail { get; set; }
    
    [StringLength(40)]
    [Unicode(false)]
    public string Name { get; set; }
    
    [StringLength(maximumLength:11, MinimumLength = 11)]
    [Unicode(false)]
    public string? ManufacturePhone { get; set; }
    
    public bool IsAvailable { get; set; }
}