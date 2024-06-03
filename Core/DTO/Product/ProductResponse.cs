namespace Core.DTO.Product;

public class ProductResponse
{
    public DateTime ProduceDate { get; set; }
    public string ManufactureEmail { get; set; }
    public string Name { get; set; }
    public string? ManufacturePhone { get; set; }
    public bool IsAvailable { get; set; }
}