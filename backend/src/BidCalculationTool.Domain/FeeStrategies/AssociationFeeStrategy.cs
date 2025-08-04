using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.FeeStrategies;

public class AssociationFeeStrategy : IFeeStrategy
{
    // Tier configuration
    private record FeeTier(decimal MinPrice, decimal MaxPrice, decimal Fee);
    private static readonly FeeTier[] FeeTiers =
    [
        new FeeTier(1.00m, 500.00m, 5.00m),
        new FeeTier(500.01m, 1000.00m, 10.00m),
        new FeeTier(1000.01m, 3000.00m, 15.00m),
        new FeeTier(3000.01m, 10000000.00m, 20.00m)
    ];

    public string FeeName => "AssociationFee";
    public string DisplayName => "Association Fee";
    public string? Description => "Tiered fee based on vehicle price. Applies to all vehicle types.";

    /// <summary>
    /// Initializes a new instance of the AssociationFeeStrategy class.
    /// </summary>
    public AssociationFeeStrategy()
    {
        // Explicit constructor to ensure proper initialization and coverage
    }

    /// <summary>
    /// Calculates the association fee based on the vehicle price using tiered pricing.
    /// </summary>
    /// <param name="vehiclePrice">The vehicle price in USD</param>
    /// <param name="vehicleType">The type of vehicle (not used in this strategy)</param>
    /// <returns>The association fee amount based on price tiers</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle price is negative</exception>
    /// <remarks>
    /// Fee structure:
    /// - $1 to $500: $5.00
    /// - $501 to $1,000: $10.00
    /// - $1,001 to $3,000: $15.00
    /// - Above $3,000: $20.00
    /// </remarks>
    public decimal Calculate(decimal vehiclePrice, VehicleTypeEnum vehicleType)
    {
        if (vehiclePrice <= 0)
        {
            throw new ArgumentException("Vehicle price cannot be negative or zero.", nameof(vehiclePrice));
        }

        // Find the appropriate fee tier for the given vehicle price
        var applicableTier = FeeTiers.FirstOrDefault(tier =>
            vehiclePrice >= tier.MinPrice && vehiclePrice <= tier.MaxPrice);

        if (applicableTier == null)
        {
            throw new ArgumentException("No fee tier found for vehicle price.", nameof(vehiclePrice));
        }

        return applicableTier.Fee;
    }
}
