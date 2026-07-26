using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Batch;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BatchController : ControllerBase
{
    private readonly IBatchService _service;

    public BatchController(IBatchService service)
    {
        _service = service;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BatchDto dto)
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

    [HttpGet("drug/{drugId}")]
    public async Task<IActionResult> GetByDrug(string drugId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetByDrugAsync(drugId, GetUserId(), page, pageSize);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBatchDto dto)
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
