namespace BidCalculationTool.Domain.Enums;

public enum VehicleType
{
    Common,
    Luxury
}

public static class VehicleTypeExtensions
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

    public static VehicleType FromString(string vehicleTypeString)
    {
        return vehicleTypeString?.ToLower() switch
        {
            "common" => VehicleType.Common,
            "luxury" => VehicleType.Luxury,
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleTypeString}", nameof(vehicleTypeString))
        };
    }
}
