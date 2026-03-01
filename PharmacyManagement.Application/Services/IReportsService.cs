// File: Application/Services/IReportsService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PharmacyManagement.Application.DTOs.Reports;

namespace PharmacyManagement.Application.Services
{
    public interface IReportsService
    {
        Task<List<SaleFinancialReportDto>> GetSalesFinancialReportAsync(DateTime? from = null, DateTime? to = null);
        Task<List<FinancialAggregateDto>> GetFinancialAggregatesAsync(string periodType);
    }
}