using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockReceiveController : ControllerBase
{
    private readonly IStockReceiveService _service;

    public StockReceiveController(IStockReceiveService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReceiveStockDto dto)
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

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetBySupplier(string supplierId)
    {
        var result = await _service.GetBySupplierAsync(supplierId, GetUserId());
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }
}
