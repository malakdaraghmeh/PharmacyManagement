using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LowStockController : ControllerBase
{
    private readonly ILowStockService _service;

    public LowStockController(ILowStockService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("critical")]
    public async Task<IActionResult> GetCritical()
    {
        var result = await _service.GetCriticalAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{drugId}")]
    public async Task<IActionResult> GetByDrug(string drugId)
    {
        var result = await _service.GetByDrugAsync(drugId, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("regenerate")]
    public async Task<IActionResult> Regenerate()
    {
        var result = await _service.RegenerateAsync(GetUserId());
        return Ok(result);
    }
}
