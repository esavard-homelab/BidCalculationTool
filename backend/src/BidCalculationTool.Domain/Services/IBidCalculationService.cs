using BidCalculationTool.Domain.Dto;

namespace BidCalculationTool.Domain.Services;

/// <summary>
/// The IBidCalculationService interface defines the contract for bid calculation services.
/// </summary>
public interface IBidCalculationService
{
    /// <summary>
    /// This method calculates the total price for a bid based on the provided request data.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The <see cref="BidCalculationResponseDto"/></returns>
    BidCalculationResponseDto CalculateTotalPrice(BidCalculationRequestDto request);
}
