using BidCalculationTool.Domain.Dto;

namespace BidCalculationTool.Domain.Services;

/// <summary>
/// Defines the contract for bid calculation services.
/// Provides operations for calculating vehicle auction bid totals including all applicable fees.
/// </summary>
public interface IBidCalculationService
{
    /// <summary>
    /// Calculates the total price for a vehicle bid including all applicable fees.
    /// </summary>
    /// <param name="request">The bid calculation request containing vehicle price and type</param>
    /// <returns>A response containing the total cost and breakdown of all fees</returns>
    /// <example>
    /// <code>
    /// var request = new BidCalculationRequestDto
    /// {
    ///     VehiclePrice = 1000m,
    ///     VehicleType = VehicleTypeEnum.Common
    /// };
    /// var result = service.CalculateTotalPrice(request);
    /// // result.TotalCost = 1180m (includes all fees)
    /// </code>
    /// </example>
    BidCalculationResponseDto CalculateTotalPrice(BidCalculationRequestDto request);
}
