using KolosB.DTOs;

namespace KolosB.Services;

public interface IMakerService
{
    Task<IEnumerable<MakerDto>> GetMakersAsync(string? name);
    Task CreateMakerAsync(CreateMakerDto dto);
}
