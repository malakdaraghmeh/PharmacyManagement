using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Application.DTOs.Drug;

public class DrugDto
{
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Packaging { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? ManufacturerId { get; set; }
    public List<string>? SupplierIds { get; set; }
    public string? Description { get; set; }
    public int MinimumStock { get; set; }
}

public class DrugResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Packaging { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? ManufacturerId { get; set; }
    public List<string> SupplierIds { get; set; } = new();
    public string? Description { get; set; }
    public int MinimumStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Rich list item returned by GET /Drug (satisfies both list and paginated consumers)
public class DrugListDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Packaging { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ManufacturerId { get; set; }
    public string? Description { get; set; }
    public int MinimumStock { get; set; }
    public int TotalStock { get; set; }
    public DrugStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BarcodeDrugResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalStock { get; set; }
}