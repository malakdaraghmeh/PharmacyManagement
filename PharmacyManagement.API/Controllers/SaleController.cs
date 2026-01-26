using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto saleDto)
    {
        var result = await _saleService.CreateSaleAsync(saleDto, GetUserId());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var result = await _saleService.GetAllSalesAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById(string id)
    {
        var result = await _saleService.GetSaleByIdAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(string id)
    {
        var result = await _saleService.DeleteSaleAsync(id, GetUserId());

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}