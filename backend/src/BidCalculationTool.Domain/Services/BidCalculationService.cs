using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.Services;

/// <summary>
/// Implementation of bid calculation service for vehicle auctions.
/// Handles the calculation of total vehicle costs including buyer fees, special fees,
/// association fees, and storage fees based on vehicle type and price.
/// </summary>
public class BidCalculationService : IBidCalculationService
{
    /// <summary>
    /// Calculates the total price for a vehicle bid including all applicable fees.
    /// </summary>
    /// <param name="request">The bid calculation request containing vehicle price and type</param>
    /// <returns>A complete response with fee breakdown and total cost</returns>
    /// <remarks>
    /// The calculation includes:
    /// - Basic buyer fee: 10% of vehicle price (with min/max limits based on type)
    /// - Special fee: 2% for Common vehicles, 4% for Luxury vehicles
    /// - Association fee: Tiered based on price ranges ($5-$20)
    /// - Storage fee: Fixed $100
    /// </remarks>
    public BidCalculationResponseDto CalculateTotalPrice(BidCalculationRequestDto request)
    {
        var basicFee = CalculateBasicBuyerFee(request.VehiclePrice, request.VehicleType);
        var specialFee = CalculateSpecialFee(request.VehiclePrice, request.VehicleType);
        var associationFee = CalculateAssociationFee(request.VehiclePrice);
        const decimal storageFee = 100.00m; // TODO: Magic number, refactor
        var totalCost = request.VehiclePrice + basicFee + specialFee + associationFee + storageFee;

        return new BidCalculationResponseDto
        {
            VehiclePrice = request.VehiclePrice,
            VehicleType = request.VehicleType,
            BasicBuyerFee = basicFee,
            SellerSpecialFee = specialFee,
            AssociationFee = associationFee,
            StorageFee = storageFee,
            TotalCost = totalCost
        };
    }

    // TODO: Refactor the methods below to use a more elegant approach, e.g. using a dictionary or a configuration file,
    // and the strategy pattern.

    /// <summary>
    /// Calculates the association fee based on the vehicle price using tiered pricing.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <returns>The association fee amount based on price tiers</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle price is negative</exception>
    /// <remarks>
    /// Fee structure:
    /// - $1 to $500: $5.00
    /// - $501 to $1,000: $10.00
    /// - $1,001 to $3,000: $15.00
    /// - Above $3,000: $20.00
    /// </remarks>
    private static decimal CalculateAssociationFee(decimal vehiclePrice)
    {
        return vehiclePrice switch
        {
            > 0 and <= 500 => 5.00m,
            > 500 and <= 1000 => 10.00m,
            > 1000 and <= 3000 => 15.00m,
            > 3000 => 20.00m,
            _ => throw new ArgumentException("Vehicle price cannot be negative.")
        };
    }

    /// <summary>
    /// Calculates the basic buyer fee as 10% of vehicle price with min/max limits based on vehicle type.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <param name="vehicleTypeEnum">The type of vehicle (Common or Luxury)</param>
    /// <returns>The calculated basic buyer fee amount</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle type is invalid</exception>
    /// <remarks>
    /// Fee limits by vehicle type:
    /// - Common: Minimum $10, Maximum $50
    /// - Luxury: Minimum $25, Maximum $200
    /// </remarks>
    private static decimal CalculateBasicBuyerFee(decimal vehiclePrice, VehicleTypeEnum vehicleTypeEnum)
    {
        return vehicleTypeEnum switch
        {
            VehicleTypeEnum.Common => Math.Max(10, Math.Min(50, vehiclePrice * 0.10m)),
            VehicleTypeEnum.Luxury => Math.Max(25, Math.Min(200, vehiclePrice * 0.10m)),
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleTypeEnum}")
        };
    }

    /// <summary>
    /// Calculates the seller's special fee as a percentage of vehicle price based on vehicle type.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <param name="vehicleTypeEnum">The type of vehicle (Common or Luxury)</param>
    /// <returns>The calculated special fee amount</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle type is invalid</exception>
    /// <remarks>
    /// Fee rates by vehicle type:
    /// - Common: 2% of vehicle price
    /// - Luxury: 4% of vehicle price
    /// </remarks>
    private static decimal CalculateSpecialFee(decimal vehiclePrice, VehicleTypeEnum vehicleTypeEnum)
    {
        return vehicleTypeEnum switch
        {
            VehicleTypeEnum.Common => vehiclePrice * 0.02m,
            VehicleTypeEnum.Luxury => vehiclePrice * 0.04m,
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleTypeEnum}")
        };
    }
}
