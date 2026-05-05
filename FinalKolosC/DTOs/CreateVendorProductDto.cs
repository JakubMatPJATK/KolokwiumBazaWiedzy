using System.Text.Json.Serialization;

namespace KolosC.DTOs;

public class CreateVendorProductDto
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public decimal PricePerUnit { get; set; }
}
