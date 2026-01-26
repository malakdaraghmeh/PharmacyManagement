using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services;
using System.Security.Claims;

namespace PharmacyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("sales-today")]
    public async Task<IActionResult> GetSalesToday()
    {
        var result = await _dashboardService.GetSalesTodayAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("invoices-today")]
    public async Task<IActionResult> GetInvoicesToday()
    {
        var result = await _dashboardService.GetInvoicesTodayAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("low-stock-drugs")]
    public async Task<IActionResult> GetLowStockDrugs()
    {
        var result = await _dashboardService.GetLowStockDrugsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("expiring-drugs")]
    public async Task<IActionResult> GetExpiringDrugs()
    {
        var result = await _dashboardService.GetExpiringDrugsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts()
    {
        var result = await _dashboardService.GetAlertsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("recent-sales")]
    public async Task<IActionResult> GetRecentSales()
    {
        var result = await _dashboardService.GetRecentSalesAsync(GetUserId());
        return Ok(result);
    }
}