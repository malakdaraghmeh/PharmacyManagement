using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Settings;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _service;

    public SettingsController(ISettingsService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("pharmacy")]
    public async Task<IActionResult> GetPharmacy()
    {
        var result = await _service.GetPharmacyAsync(GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("pharmacy")]
    public async Task<IActionResult> UpdatePharmacy([FromBody] UpdatePharmacySettingsDto dto)
    {
        var result = await _service.UpdatePharmacyAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("system")]
    public async Task<IActionResult> GetSystem()
    {
        var result = await _service.GetSystemAsync(GetUserId());
        return Ok(result);
    }

    [HttpPut("system")]
    public async Task<IActionResult> UpdateSystem([FromBody] SystemSettingsDto dto)
    {
        var result = await _service.UpdateSystemAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
