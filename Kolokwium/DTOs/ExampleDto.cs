using System.ComponentModel.DataAnnotations;

namespace Kolokwium.DTOs;

public record ExampleResponseDto(int Id, string Name);

public record ExampleRequest(
    [Required, MaxLength(100)] string Name);
