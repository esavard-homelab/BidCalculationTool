namespace BidCalculationTool.Domain.Enums;

/// <summary>
/// Represents the different types of vehicles available for bid calculations.
/// It includes common and luxury vehicle types.
/// </summary>
public enum VehicleTypeEnum
{
    Common,
    Luxury
}

/// <summary>
/// This static class provides extension methods for the VehicleTypeEnum enum.
/// </summary>
public static class VehicleTypeExtensions
{
    /// <summary>
    /// Converts the vehicle type enum value to its display string representation.
    /// </summary>
    /// <param name="vehicleTypeEnum">The vehicle type enum to convert.</param>
    /// <returns>A string representation of the vehicle type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the vehicle type is not supported.</exception>
    public static string ToDisplayString(this VehicleTypeEnum vehicleTypeEnum)
    {
        return vehicleTypeEnum switch
        {
            VehicleTypeEnum.Common => "Common",
            VehicleTypeEnum.Luxury => "Luxury",
            _ => throw new ArgumentOutOfRangeException(nameof(vehicleTypeEnum), vehicleTypeEnum, null)
        };
    }

    /// <summary>
    /// Converts a string representation to the corresponding VehicleTypeEnum.
    /// Supports both "Common"/"Luxury" (display format) and "COMMON"/"LUXURY" (API format).
    /// </summary>
    /// <param name="vehicleTypeString">The string to convert (case-insensitive).</param>
    /// <returns>The corresponding VehicleTypeEnum.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid vehicle type.</exception>
    public static VehicleTypeEnum FromString(string vehicleTypeString)
    {
        if (string.IsNullOrWhiteSpace(vehicleTypeString))
            throw new ArgumentException("Vehicle type string cannot be null or empty.", nameof(vehicleTypeString));

        return vehicleTypeString.ToUpperInvariant() switch
        {
            "COMMON" => VehicleTypeEnum.Common,
            "LUXURY" => VehicleTypeEnum.Luxury,
            _ => throw new ArgumentException($"Invalid vehicle type: '{vehicleTypeString}'. Valid values are: 'Common', 'Luxury', 'COMMON', 'LUXURY'.", nameof(vehicleTypeString))
        };
    }
}
