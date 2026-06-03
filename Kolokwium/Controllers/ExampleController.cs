using Kolokwium.DTOs;
using Kolokwium.Exceptions;
using Kolokwium.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController(IDbService service) : ControllerBase
{
    // GET /api/example
    // GET /api/example?name=test
    [HttpGet]
    public Task<IReadOnlyList<ExampleResponseDto>> GetAll(
        [FromQuery] string? name,
        CancellationToken cancellationToken)
        => service.GetAllExamplesAsync(name, cancellationToken);

    // GET /api/example/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExampleResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetExampleAsync(id, cancellationToken));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExampleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await service.AddExampleAsync(request, cancellationToken);
            return Created();
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] ExampleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await service.UpdateExampleAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await service.DeleteExampleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}
