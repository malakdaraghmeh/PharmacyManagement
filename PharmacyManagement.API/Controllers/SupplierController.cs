using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Supplier;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _service;

    public SupplierController(ISupplierService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SupplierDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null)
    {
        var result = await _service.GetAllAsync(GetUserId(), page, pageSize, name);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSupplierDto dto)
    {
        var result = await _service.UpdateAsync(id, dto, GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id, GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }
}
