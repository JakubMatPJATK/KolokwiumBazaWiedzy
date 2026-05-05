using System.Text.Json.Serialization;

namespace KolosB.DTOs;

public class ProductTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
