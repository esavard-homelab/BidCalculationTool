namespace BidCalculationTool.Domain.Dto;

/// <summary>
/// This DTO is used to represent a vehicle type in the system.
/// It includes a value and a label for the vehicle type.
/// This is typically used in the dropdown in the UI.
/// </summary>
public record VehicleTypeDto
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
}
