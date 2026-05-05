using System.Text.Json.Serialization;

namespace KolosB.DTOs;

public class MakerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
}
