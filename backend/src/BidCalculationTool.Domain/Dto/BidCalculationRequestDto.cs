using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.Dto;

public record BidCalculationRequestDto
{
    public decimal VehiclePrice { get; set; }
    public required string VehicleType { get; set; }
}
