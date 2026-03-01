public class SaleFinancialReportDto
{
    public required string SaleId { get; set; }
    public required string CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public required string PaymentMethod { get; set; }

    public decimal TotalProfit { get; set; }
    public decimal TotalLoss { get; set; }
    public decimal NetProfit => TotalProfit - TotalLoss;

    public decimal CashCollected { get; set; }
    public decimal CreditCollected { get; set; }

    public List<SaleItemFinancialDto> Items { get; set; } = new List<SaleItemFinancialDto>();
}

public class SaleItemFinancialDto
{
    public required string DrugName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalProfitOrLoss { get; set; }
}