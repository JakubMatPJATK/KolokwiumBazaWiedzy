using System.Text.Json.Serialization;

namespace PJATK-APBD-Kol1-Gr17c-s34002.DTOs;

public class CreateVendorProductDto
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public decimal PricePerUnit { get; set; }
}
