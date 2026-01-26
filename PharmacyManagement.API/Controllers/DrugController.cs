using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Drug;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DrugController : ControllerBase
{
    private readonly IDrugService _drugService;

    public DrugController(IDrugService drugService)
    {
        _drugService = drugService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> CreateDrug([FromBody] DrugDto drugDto)
    {
        var result = await _drugService.CreateDrugAsync(drugDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDrugs()
    {
        var result = await _drugService.GetAllDrugsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDrugById(string id)
    {
        var result = await _drugService.GetDrugByIdAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetDrugByBarcode(string barcode)
    {
        var result = await _drugService.GetDrugByBarcodeAsync(barcode, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDrug(string id, [FromBody] DrugDto drugDto)
    {
        var result = await _drugService.UpdateDrugAsync(id, drugDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeDrugStatus(string id)
    {
        var result = await _drugService.ChangeDrugStatusAsync(id, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrug(string id)
    {
        var result = await _drugService.DeleteDrugAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}