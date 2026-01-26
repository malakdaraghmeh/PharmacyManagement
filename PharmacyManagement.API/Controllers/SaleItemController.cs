using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SaleItemController : ControllerBase
{
    private readonly ISaleItemService _saleItemService;

    public SaleItemController(ISaleItemService saleItemService)
    {
        _saleItemService = saleItemService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> CreateSaleItem([FromBody] SaleItemDto saleItemDto)
    {
        var result = await _saleItemService.CreateSaleItemAsync(saleItemDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSaleItems()
    {
        var result = await _saleItemService.GetAllSaleItemsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleItemById(string id)
    {
        var result = await _saleItemService.GetSaleItemByIdAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSaleItem(string id, [FromBody] SaleItemDto saleItemDto)
    {
        var result = await _saleItemService.UpdateSaleItemAsync(id, saleItemDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSaleItem(string id)
    {
        var result = await _saleItemService.DeleteSaleItemAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}