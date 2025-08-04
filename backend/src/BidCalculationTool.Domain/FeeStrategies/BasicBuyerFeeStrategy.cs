using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.FeeStrategies;

public class BasicBuyerFeeStrategy : IFeeStrategy
{
    // Fee calculation constants
    private const decimal FeePercentage = 0.10m; // 10%

    // Common vehicle fee limits
    private const decimal CommonMinimumFee = 10.00m;
    private const decimal CommonMaximumFee = 50.00m;

    // Luxury vehicle fee limits
    private const decimal LuxuryMinimumFee = 25.00m;
    private const decimal LuxuryMaximumFee = 200.00m;

    public string FeeName => "BasicBuyerFee";
    public string DisplayName => "Basic Buyer Fee";
    public string? Description => "10% of vehicle price with minimum and maximum limits based on vehicle type";

    /// <summary>
    /// Initializes a new instance of the BasicBuyerFeeStrategy class.
    /// </summary>
    public BasicBuyerFeeStrategy()
    {
        // Explicit constructor to ensure proper initialization and coverage
    }

    /// <summary>
    /// Calculates the basic buyer fee as 10% of vehicle price with min/max limits based on vehicle type.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <param name="vehicleType">The type of vehicle (Common or Luxury)</param>
    /// <returns>The calculated basic buyer fee amount</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle type is invalid</exception>
    /// <remarks>
    /// Fee limits by vehicle type:
    /// - Common: Minimum $10, Maximum $50
    /// - Luxury: Minimum $25, Maximum $200
    /// </remarks>
    public decimal Calculate(decimal vehiclePrice, VehicleTypeEnum vehicleType)
    {
        var calculatedFee = vehiclePrice * FeePercentage;

        return vehicleType switch
        {
            VehicleTypeEnum.Common => Math.Max(CommonMinimumFee, Math.Min(CommonMaximumFee, calculatedFee)),
            VehicleTypeEnum.Luxury => Math.Max(LuxuryMinimumFee, Math.Min(LuxuryMaximumFee, calculatedFee)),
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleType}")
        };
    }
}
