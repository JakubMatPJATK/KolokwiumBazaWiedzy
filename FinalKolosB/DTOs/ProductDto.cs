using System.Text.Json.Serialization;

namespace KolosB.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal StrickerPrice { get; set; }
    public ProductTypeDto ProductType { get; set; } = null!;
    public IEnumerable<VendorDto> Vendors { get; set; } = new List<VendorDto>();
}
