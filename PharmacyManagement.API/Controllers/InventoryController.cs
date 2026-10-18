using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _service.GetSummaryAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockItems([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetLowStockItemsAsync(GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("expiry-alerts")]
    public async Task<IActionResult> GetExpiryAlerts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetExpiryAlertsAsync(GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("top-movers")]
    public async Task<IActionResult> GetTopMovers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetTopMoversAsync(GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetRecommendationsAsync(GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("risk-score")]
    public async Task<IActionResult> GetRiskScore()
    {
        var result = await _service.GetRiskScoreAsync(GetUserId());
        return Ok(result);
    }
}
