using PharmacyManagement.Application.DTOs.Reports;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagement.Application.Services.Implementation
{
    public class ReportsService : IReportsService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IDrugRepository _drugRepository;
        private readonly ICreditRecordRepository _creditRepository;

        public ReportsService(
            ISaleRepository saleRepository, 
            IDrugRepository drugRepository, 
            ICreditRecordRepository creditRepository)
        {
            _saleRepository = saleRepository;
            _drugRepository = drugRepository;
            _creditRepository = creditRepository;
        }

        public async Task<List<SaleFinancialReportDto>> GetSalesFinancialReportAsync(DateTime? from = null, DateTime? to = null)
        {
            var salesQuery = _saleRepository.GetAllSalesWithItemsAsyncQueryable();

            if (from.HasValue)
                salesQuery = salesQuery.Where(s => s.CreatedAt >= from.Value);

            if (to.HasValue)
                salesQuery = salesQuery.Where(s => s.CreatedAt <= to.Value);

            var sales = await salesQuery.ToListAsync();
            var drugs = await _drugRepository.GetAllAsync();

            var report = new List<SaleFinancialReportDto>();

            foreach (var sale in sales)
            {
                var saleReport = new SaleFinancialReportDto
                {
                    SaleId = sale.Id,
                    CustomerName = sale.CustomerName,
                    SaleDate = sale.CreatedAt,
                    PaymentMethod = sale.PaymentMethod
                };

                foreach (var item in sale.SaleItems)
                {
                    var drug = drugs.FirstOrDefault(d => d.Id == item.DrugId);
                    if (drug == null) continue;

                    var profitOrLoss = (item.UnitPrice - drug.CostPrice) * item.Quantity;

                    saleReport.Items.Add(new SaleItemFinancialDto
                    {
                        DrugName = item.DrugName,
                        UnitPrice = item.UnitPrice,
                        CostPrice = drug.CostPrice,
                        Quantity = item.Quantity,
                        TotalProfitOrLoss = profitOrLoss
                    });

                    if (profitOrLoss >= 0)
                        saleReport.TotalProfit += profitOrLoss;
                    else
                        saleReport.TotalLoss += Math.Abs(profitOrLoss);
                }

                if (sale.PaymentMethod == "Cash")
                    saleReport.CashCollected = sale.NetAmount;
                else
                    saleReport.CreditCollected = sale.NetAmount;

                report.Add(saleReport);
            }

            return report;
        }

        public async Task<List<FinancialAggregateDto>> GetFinancialAggregatesAsync(string periodType)
        {
            var sales = await _saleRepository.GetAllSalesWithItemsAsync();
            var drugs = await _drugRepository.GetAllAsync();

            var saleReports = new List<(DateTime Date, decimal Profit, decimal Loss, decimal Cash, decimal Credit)>();

            foreach (var sale in sales)
            {
                decimal totalProfit = 0, totalLoss = 0;

                foreach (var item in sale.SaleItems)
                {
                    var drug = drugs.FirstOrDefault(d => d.Id == item.DrugId);
                    if (drug == null) continue;

                    var profitOrLoss = (item.UnitPrice - drug.CostPrice) * item.Quantity;

                    if (profitOrLoss >= 0)
                        totalProfit += profitOrLoss;
                    else
                        totalLoss += Math.Abs(profitOrLoss);
                }

                decimal cash = sale.PaymentMethod == "Cash" ? sale.NetAmount : 0;
                decimal credit = sale.PaymentMethod != "Cash" ? sale.NetAmount : 0;

                saleReports.Add((sale.CreatedAt, totalProfit, totalLoss, cash, credit));
            }

            IEnumerable<IGrouping<object, (DateTime Date, decimal Profit, decimal Loss, decimal Cash, decimal Credit)>> grouped;

            switch (periodType.ToLower())
            {
                case "daily":
                    grouped = saleReports.GroupBy(s => (object)s.Date.Date);
                    break;
                case "monthly":
                    grouped = saleReports.GroupBy(s => (object)new { s.Date.Year, s.Date.Month });
                    break;
                case "annual":
                    grouped = saleReports.GroupBy(s => (object)s.Date.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid periodType. Use 'daily', 'monthly', or 'annual'.");
            }

            var aggregates = new List<FinancialAggregateDto>();

            foreach (var group in grouped)
            {
                string periodLabel = periodType.ToLower() switch
                {
                    "daily" => ((DateTime)group.Key).ToString("yyyy-MM-dd"),
                    "monthly" => group.Key is { } key && key.GetType().GetProperty("Year") != null
                        ? new DateTime(
                            (int)key.GetType().GetProperty("Year")!.GetValue(key)!,
                            (int)key.GetType().GetProperty("Month")!.GetValue(key)!,
                            1).ToString("MMMM yyyy")
                        : "",
                    "annual" => group.Key.ToString(),
                    _ => ""
                };

                aggregates.Add(new FinancialAggregateDto
                {
                    Period = periodLabel,
                    TotalProfit = group.Sum(g => g.Profit),
                    TotalLoss = group.Sum(g => g.Loss),
                    Cash = group.Sum(g => g.Cash),
                    CreditReceived = group.Sum(g => g.Credit)
                });
            }

            return aggregates;
        }
    }
}