using KolosC.DTOs;

namespace KolosC.Services;

public interface IVendorService
{
    Task<VendorDto> GetVendorByCodeAsync(string code);
    Task CreateVendorAsync(CreateVendorDto dto);
}
