namespace PharmacyManagement.Application.DTOs.Manufacturer;

public class ManufacturerDto
{
    public string Name { get; set; } = string.Empty;
}

public class ManufacturerResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
