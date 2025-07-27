using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.Dto;

/// <summary>
/// This DTO is used to encapsulate the response data for a bid calculation.
/// It includes the vehicle price, type, various fees, and the total cost.
/// </summary>
public record BidCalculationResponseDto
{
    public required decimal VehiclePrice { get; init; }
    public required VehicleTypeEnum VehicleType { get; init; }
    public required decimal BasicBuyerFee { get; init; }
    public required decimal SellerSpecialFee { get; init; }
    public required decimal AssociationFee { get; init; }
    public required decimal StorageFee { get; init; }
    public required decimal TotalCost { get; init; }
}
