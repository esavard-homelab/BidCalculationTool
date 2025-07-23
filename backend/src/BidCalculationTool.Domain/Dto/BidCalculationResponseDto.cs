namespace BidCalculationTool.Domain.Dto;

/// <summary>
/// This DTO is used to encapsulate the response data for a bid calculation.
/// It includes the vehicle price, type, various fees, and the total cost.
/// </summary>
public record BidCalculationResponseDto
{
    public decimal VehiclePrice { get; set; }
    public required string VehicleType { get; set; }
    public decimal BasicBuyerFee { get; init; }
    public decimal SellerSpecialFee { get; init; }
    public decimal AssociationFee { get; init; }
    public decimal StorageFee { get; init; }
    public decimal TotalCost { get; init; }
}
