using System.Text.Json.Serialization;

namespace KolosC.DTOs;

public class CreateVendorDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IEnumerable<CreateVendorProductDto>? Products { get; set; }
}
