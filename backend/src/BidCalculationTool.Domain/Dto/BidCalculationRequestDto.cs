using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.Dto;

/// <summary>
/// Bid calculation request data transfer object.
/// This DTO is used to encapsulate the data required for calculating a bid.
/// It includes the vehicle price and the type of vehicle.
/// </summary>
public record BidCalculationRequestDto
{
    public required decimal VehiclePrice { get; init; }
    public required VehicleTypeEnum VehicleType { get; init; }
}
