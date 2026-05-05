using Microsoft.AspNetCore.Mvc;
using KolosB.DTOs;
using KolosB.Exceptions;
using KolosB.Services;

namespace KolosB.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MakersController(IMakerService makerSvc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMakers([FromQuery] string? name)
    {
        var list = await makerSvc.GetMakersAsync(name);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMaker([FromBody] CreateMakerDto dto)
    {
        try
        {
            await makerSvc.CreateMakerAsync(dto);
            return StatusCode(201);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
