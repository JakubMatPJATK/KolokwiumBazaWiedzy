using Kolokwium.DTOs;

namespace Kolokwium.Services;

// Jedna warstwa serwisu do komunikacji z bazą (jak w przykładzie PJATK).
// Rozszerz interfejs o metody z PDF kolokwium.
public interface IDbService
{
    Task<IReadOnlyList<ExampleResponseDto>> GetAllExamplesAsync(string? name, CancellationToken cancellationToken);
    Task<ExampleResponseDto> GetExampleAsync(int id, CancellationToken cancellationToken);
    Task AddExampleAsync(ExampleRequest request, CancellationToken cancellationToken);
    Task UpdateExampleAsync(int id, ExampleRequest request, CancellationToken cancellationToken);
    Task DeleteExampleAsync(int id, CancellationToken cancellationToken);
}
