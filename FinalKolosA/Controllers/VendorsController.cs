using Microsoft.AspNetCore.Mvc;
using PJATK-APBD-Kol1-Gr17c-s34002.DTOs;
using PJATK-APBD-Kol1-Gr17c-s34002.Exceptions;
using PJATK-APBD-Kol1-Gr17c-s34002.Services;

namespace PJATK-APBD-Kol1-Gr17c-s34002.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController(IVendorService _svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetVendorsAsync([FromQuery] string? name)
    {
        var res = await _svc.GetVendorsAsync(name);
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendorAsync([FromBody] CreateVendorDto dto)
    {
        try
        {
            await _svc.CreateVendorAsync(dto);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
}
