namespace PharmacyManagement.Application.DTOs.Drug;

public class DrugDto
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class DrugResponseDto : DrugDto
{
    public string Id { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BarcodeDrugResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }
}