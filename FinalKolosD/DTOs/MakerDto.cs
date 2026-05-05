using System.Text.Json.Serialization;

namespace KolosD.DTOs;

public class MakerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
}
