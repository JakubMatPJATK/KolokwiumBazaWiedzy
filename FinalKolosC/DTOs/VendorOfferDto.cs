using System.Text.Json.Serialization;

namespace KolosC.DTOs;

public class VendorOfferDto
{
    public int Amount { get; set; }
    public decimal PricePerUnit { get; set; }
}
