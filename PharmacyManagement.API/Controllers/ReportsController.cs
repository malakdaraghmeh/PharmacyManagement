using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Application.Services; // for IReportsService
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet("sales-financial-report")]
    public async Task<IActionResult> GetSalesFinancialReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var report = await _reportsService.GetSalesFinancialReportAsync(from, to);
        return Ok(report);
    }

    [HttpGet("financial-aggregates")]
    public async Task<IActionResult> GetFinancialAggregates([FromQuery] string periodType)
    {
        var aggregates = await _reportsService.GetFinancialAggregatesAsync(periodType);
        return Ok(aggregates);
    }
}