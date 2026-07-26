using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementController : ControllerBase
{
    private readonly IStockMovementService _service;

    public StockMovementController(IStockMovementService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpGet("drug/{drugId}")]
    public async Task<IActionResult> GetByDrug(string drugId)
    {
        var result = await _service.GetByDrugAsync(drugId, GetUserId());
        return Ok(result);
    }

    [HttpGet("batch/{batchId}")]
    public async Task<IActionResult> GetByBatch(string batchId)
    {
        var result = await _service.GetByBatchAsync(batchId, GetUserId());
        return Ok(result);
    }
}
