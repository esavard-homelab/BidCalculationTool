using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.FeeStrategies;

public class FixedStorageFeeStrategy : IFeeStrategy
{
    // Fixed storage fee amount
    private const decimal FixedStorageAmount = 100.00m;

    public string FeeName => "StorageFee";
    public string DisplayName => "Storage Fee";
    public string? Description => "Fixed storage fee applied to all vehicle auctions";

    /// <summary>
    /// Initializes a new instance of the FixedStorageFeeStrategy class.
    /// </summary>
    public FixedStorageFeeStrategy()
    {
        // Explicit constructor to ensure proper initialization and coverage
    }

    /// <summary>
    /// Calculates a fixed storage fee regardless of vehicle price or type.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price (not used in this strategy)</param>
    /// <param name="vehicleType">The vehicle type (not used in this strategy)</param>
    /// <returns>The fixed storage fee amount</returns>
    public decimal Calculate(decimal vehiclePrice, VehicleTypeEnum vehicleType) => FixedStorageAmount;
}
