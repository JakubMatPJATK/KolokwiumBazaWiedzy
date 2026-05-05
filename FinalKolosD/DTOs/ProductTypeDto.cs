using System.Text.Json.Serialization;

namespace KolosD.DTOs;

public class ProductTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
