namespace BidCalculationTool.Domain.Dto;

/// <summary>
/// Represents a single fee item in the breakdown of total costs.
/// This allows for dynamic fee structures without frontend coupling.
/// </summary>
public record FeeBreakdownItem
{
    /// <summary>
    /// Internal name/key of the fee (e.g., "BasicBuyerFee")
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name (e.g., "Basic Buyer Fee")
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The calculated fee amount
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Optional description for tooltip or additional information
    /// </summary>
    public string? Description { get; init; }
}
