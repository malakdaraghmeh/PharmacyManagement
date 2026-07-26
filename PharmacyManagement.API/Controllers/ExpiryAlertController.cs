using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpiryAlertController : ControllerBase
{
    private readonly IExpiryAlertService _service;

    public ExpiryAlertController(IExpiryAlertService service)
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("drug/{drugId}")]
    public async Task<IActionResult> GetByDrug(string drugId)
    {
        var result = await _service.GetByDrugAsync(drugId, GetUserId());
        return Ok(result);
    }

    [HttpPost("{id}/acknowledge")]
    public async Task<IActionResult> Acknowledge(string id)
    {
        var result = await _service.AcknowledgeAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }
}
