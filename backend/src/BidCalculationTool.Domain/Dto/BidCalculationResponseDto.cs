namespace BidCalculationTool.Domain.Dto;

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
