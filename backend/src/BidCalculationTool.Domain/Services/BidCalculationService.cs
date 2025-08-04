using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.FeeStrategies;

namespace BidCalculationTool.Domain.Services;

/// <summary>
/// Implementation of bid calculation service for vehicle auctions.
/// Handles the calculation of total vehicle costs including buyer fees, special fees,
/// association fees, and storage fees based on vehicle type and price.
/// </summary>
public class BidCalculationService : IBidCalculationService
{
    private readonly List<IFeeStrategy> _feeStrategies;

    public BidCalculationService(IEnumerable<IFeeStrategy> feeStrategies)
    {
        _feeStrategies = feeStrategies.ToList();
        if (_feeStrategies.Count == 0)
        {
            throw new ArgumentException("No fee strategies registered. Please register at least one fee strategy.");
        }
    }

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
        var fees = new Dictionary<string, decimal>();
        var feeBreakdown = new List<FeeBreakdownItem>();
        decimal totalFees = 0;

        foreach (var strategy in _feeStrategies)
        {
            var fee = strategy.Calculate(request.VehiclePrice, request.VehicleType);
            fees[strategy.FeeName] = fee;
            totalFees += fee;

            // Create dynamic fee breakdown item
            feeBreakdown.Add(new FeeBreakdownItem
            {
                Name = strategy.FeeName,
                DisplayName = strategy.DisplayName,
                Amount = fee,
                Description = strategy.Description
            });
        }

        return new BidCalculationResponseDto
        {
            VehiclePrice = request.VehiclePrice,
            VehicleType = request.VehicleType,
            FeeBreakdown = feeBreakdown,
            TotalCost = request.VehiclePrice + totalFees
        };
    }
}
