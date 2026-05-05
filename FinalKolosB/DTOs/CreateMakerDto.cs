using System.Text.Json.Serialization;

namespace KolosB.DTOs;

public class CreateMakerDto
{
    public string Name { get; set; } = null!;
    public IEnumerable<CreateProductDto>? Products { get; set; }
}
