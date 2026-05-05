using KolosD.DTOs;

namespace KolosD.Services;

public interface IMakerService
{
    Task<MakerDto> GetMakerByIdAsync(int id);
    Task CreateMakerAsync(CreateMakerDto dto);
}
