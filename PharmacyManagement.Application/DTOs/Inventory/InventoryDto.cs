using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Application.DTOs.Inventory;

public class LowStockDto
{
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int TotalStock { get; set; }
    public int MinimumStock { get; set; }
    public LowStockStatus Status { get; set; }
    public LowStockSeverity Severity { get; set; }
    public double AverageDailySales { get; set; }
    public double EstimatedDaysLeft { get; set; }
    public int RecommendedReorderQuantity { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public DateTime? LastRestockDate { get; set; }
}

public class ExpiryAlertDto
{
    public string Id { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int RemainingQuantity { get; set; }
    public int DaysToExpire { get; set; }
    public ExpiryStatus Status { get; set; }
    public ExpirySeverity Severity { get; set; }
    public decimal EstimatedLossValue { get; set; }
    public ExpiryAction RecommendedAction { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TopMovingDrugDto
{
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public string Trend { get; set; } = "stable"; // up, down, stable
    public double DemandScore { get; set; }
}

public class InventoryRecommendationDto
{
    public RecommendationType Type { get; set; }
    public string? DrugId { get; set; }
    public string? DrugName { get; set; }
    public string Message { get; set; } = string.Empty;
    public RecommendationPriority Priority { get; set; }
    public decimal? EstimatedImpact { get; set; }
}

public class InventorySummaryDto
{
    public int TotalDrugs { get; set; }
    public int TotalBatches { get; set; }
    public int TotalStockItems { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<LowStockDto> LowStockItems { get; set; } = new();
    public List<ExpiryAlertDto> ExpiryAlerts { get; set; } = new();
    public int OutOfStockDrugsCount { get; set; }
    public int CriticalAlertsCount { get; set; }
    public double InventoryRiskScore { get; set; }
    public List<TopMovingDrugDto> TopMovingDrugs { get; set; } = new();
    public List<InventoryRecommendationDto> RecommendedActions { get; set; } = new();
}
