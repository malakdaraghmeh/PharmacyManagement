using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.CreditRecord;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreditRecordController : ControllerBase
{
    private readonly ICreditRecordService _creditRecordService;

    public CreditRecordController(ICreditRecordService creditRecordService)
    {
        _creditRecordService = creditRecordService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> CreateCreditRecord([FromBody] CreditRecordDto creditRecordDto)
    {
        var result = await _creditRecordService.CreateCreditRecordAsync(creditRecordDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCreditRecords()
    {
        var result = await _creditRecordService.GetAllCreditRecordsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCreditRecordById(string id)
    {
        var result = await _creditRecordService.GetCreditRecordByIdAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCreditRecord(string id, [FromBody] CreditRecordDto creditRecordDto)
    {
        var result = await _creditRecordService.UpdateCreditRecordAsync(id, creditRecordDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCreditRecord(string id)
    {
        var result = await _creditRecordService.DeleteCreditRecordAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}