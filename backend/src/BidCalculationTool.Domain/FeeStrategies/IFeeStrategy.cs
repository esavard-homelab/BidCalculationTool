using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Domain.FeeStrategies;

public interface IFeeStrategy
{
    decimal Calculate(decimal vehiclePrice, VehicleTypeEnum vehicleType);
    string FeeName { get; }
    string DisplayName { get; }
    string? Description { get; }
}
