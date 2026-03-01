namespace PharmacyManagement.Application.DTOs.Reports
{
    public class FinancialAggregateDto
    {
        public required string Period { get; set; } // e.g., "2026-03-01", "March 2026", "2026"
        public decimal TotalProfit { get; set; }
        public decimal TotalLoss { get; set; }
        public decimal NetProfit => TotalProfit - TotalLoss;
        public decimal Cash { get; set; }
        public decimal CreditReceived { get; set; }
    }
}