using System.Text.Json.Serialization;

namespace KolosD.DTOs;

public class CreateMakerDto
{
    public string Name { get; set; } = null!;
    public IEnumerable<CreateProductDto>? Products { get; set; }
}
