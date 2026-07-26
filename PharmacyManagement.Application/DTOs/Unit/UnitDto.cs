namespace PharmacyManagement.Application.DTOs.Unit;

public class UnitDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
}

public class UpdateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
}

public class UnitResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; }
}
