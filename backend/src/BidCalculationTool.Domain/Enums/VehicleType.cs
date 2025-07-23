namespace BidCalculationTool.Domain.Enums;

/// <summary>
/// Represents the different types of vehicles available for bid calculations.
/// It includes common and luxury vehicle types.
/// </summary>
public enum VehicleType
{
    Common,
    Luxury
}

/// <summary>
/// This static class provides extension methods for the VehicleType enum.
/// </summary>
public static class VehicleTypeExtensions
{
    /// <summary>
    /// Converts the vehicle type enum value to its display string representation.
    /// </summary>
    /// <param name="vehicleType">The vehicle type enum to convert.</param>
    /// <returns>A string representation of the vehicle type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the vehicle type is not supported.</exception>
    public static string ToDisplayString(this VehicleType vehicleType)
    {
        return vehicleType switch
        {
            VehicleType.Common => "Common",
            VehicleType.Luxury => "Luxury",
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null)
        };
    }

    // TODO: Add a method to convert from string to VehicleType enum (not required for now)
}
