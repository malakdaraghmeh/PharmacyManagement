namespace PharmacyManagement.Application.DTOs.Dashboard;

public class SalesTodayDto
{
    public decimal Total { get; set; }
}

public class InvoicesTodayDto
{
    public int Count { get; set; }
}

public class LowStockDrugDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class ExpiringDrugDto
{
    public string Name { get; set; } = string.Empty;
    public int DaysLeft { get; set; }
}

public class AlertDto
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class RecentSaleDto
{
    public string Time { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}