using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.FeeStrategies;

public class SpecialFeeStrategy : IFeeStrategy
{
    // Seller's special fee percentages
    private const decimal CommonVehicleFeePercentage = 0.02m; // 2%
    private const decimal LuxuryVehicleFeePercentage = 0.04m; // 4%

    public string FeeName => "SpecialFee";
    public string DisplayName => "Seller's Special Fee";
    public string? Description => "Percentage-based fee: 2% for Common vehicles, 4% for Luxury vehicles";

    /// <summary>
    /// Initializes a new instance of the SpecialFeeStrategy class.
    /// </summary>
    public SpecialFeeStrategy()
    {
        // Explicit constructor to ensure proper initialization and coverage
    }

    /// <summary>
    /// Calculates the seller's special fee as a percentage of vehicle price based on vehicle type.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <param name="vehicleType">The type of vehicle (Common or Luxury)</param>
    /// <returns>The calculated special fee amount</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle type is invalid</exception>
    /// <remarks>
    /// Fee rates by vehicle type:
    /// - Common: 2% of vehicle price
    /// - Luxury: 4% of vehicle price
    /// </remarks>
    public decimal Calculate(decimal vehiclePrice, VehicleTypeEnum vehicleType)
    {
        return vehicleType switch
        {
            VehicleTypeEnum.Common => vehiclePrice * CommonVehicleFeePercentage,
            VehicleTypeEnum.Luxury => vehiclePrice * LuxuryVehicleFeePercentage,
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleType}")
        };
    }
}
