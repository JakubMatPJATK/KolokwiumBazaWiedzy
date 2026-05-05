using Microsoft.AspNetCore.Mvc;
using KolosC.DTOs;
using KolosC.Exceptions;
using KolosC.Services;

namespace KolosC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController(IVendorService vendorSvc) : ControllerBase
{
    [HttpGet("{code}")]
    public async Task<IActionResult> GetVendor([FromRoute] string code)
    {
        try
        {
            var v = await vendorSvc.GetVendorByCodeAsync(code);
            return Ok(v);
        }
        catch (NotFoundException nf)
        {
            return NotFound(nf.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddVendor([FromBody] CreateVendorDto dto)
    {
        try
        {
            await vendorSvc.CreateVendorAsync(dto);
            return Created();
        }
        catch (NotFoundException nf)
        {
            return NotFound(nf.Message);
        }
        catch (BadRequestException br)
        {
            return BadRequest(br.Message);
        }
    }
}
