namespace BidCalculationTool.Domain.Enums;

public enum VehicleType
{
    Common,
    Luxury
}

public static partial class VehicleTypeExtensions
{
    public static string ToDisplayString(this VehicleType vehicleType)
    {
        return vehicleType switch
        {
            VehicleType.Common => "Common",
            VehicleType.Luxury => "Luxury",
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null)
        };
    }
}
