using System.Text.Json.Serialization;

namespace KolosC.DTOs;

public class VendorDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
}
