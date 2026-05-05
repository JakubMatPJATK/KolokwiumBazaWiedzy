using System.Text.Json.Serialization;

namespace PJATK-APBD-Kol1-Gr17c-s34002.DTOs;

public class VendorDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
}
