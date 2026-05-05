using System.Text.Json.Serialization;

namespace KolosC.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal StrickerPrice { get; set; }
    public ProductTypeDto ProductType { get; set; } = null!;
    public MakerDto Maker { get; set; } = null!;
    public VendorOfferDto VendorOffer { get; set; } = null!;
}
