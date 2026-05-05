using PJATK-APBD-Kol1-Gr17c-s34002.DTOs;

namespace PJATK-APBD-Kol1-Gr17c-s34002.Services;

public interface IVendorService
{
    Task<IEnumerable<VendorDto>> GetVendorsAsync(string? name);
    Task CreateVendorAsync(CreateVendorDto dto);
}
