using Microsoft.AspNetCore.Mvc;
using PJATK-APBD-Kol1-Gr17c-s34002.DTOs;
using PJATK-APBD-Kol1-Gr17c-s34002.Exceptions;
using PJATK-APBD-Kol1-Gr17c-s34002.Services;

namespace PJATK-APBD-Kol1-Gr17c-s34002.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MakersController(IMakerService s) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var m = await s.GetMakerByIdAsync(id);
            return Ok(m);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMakerDto dto)
    {
        try
        {
            await s.CreateMakerAsync(dto);
            return Created();
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
