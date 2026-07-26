using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockAdjustmentController : ControllerBase
{
    private readonly IStockAdjustmentService _service;

    public StockAdjustmentController(IStockAdjustmentService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StockAdjustmentDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

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

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(string id)
    {
        var result = await _service.ApproveAsync(id, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(string id)
    {
        var result = await _service.RejectAsync(id, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
