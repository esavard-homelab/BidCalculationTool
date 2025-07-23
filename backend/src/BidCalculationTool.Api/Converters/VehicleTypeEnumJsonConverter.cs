using BidCalculationTool.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BidCalculationTool.Api.Converters;

/// <summary>
/// JSON converter for VehicleTypeEnum that handles string to enum conversion.
/// Supports both API format (COMMON, LUXURY) and display format (Common, Luxury).
/// </summary>
public class VehicleTypeEnumJsonConverter : JsonConverter<VehicleTypeEnum>
{
    /// <summary>
    /// Reads and converts a JSON string value to VehicleTypeEnum.
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">Type to convert to</param>
    /// <param name="options">JSON serializer options</param>
    /// <returns>The converted VehicleTypeEnum value</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is invalid</exception>
    public override VehicleTypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string value for VehicleTypeEnum, got {reader.TokenType}");
        }

        var stringValue = reader.GetString();

        if (string.IsNullOrWhiteSpace(stringValue))
        {
            throw new JsonException("Vehicle type cannot be null or empty");
        }

        try
        {
            return VehicleTypeExtensions.FromString(stringValue);
        }
        catch (ArgumentException ex)
        {
            throw new JsonException($"Invalid vehicle type: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Writes a VehicleTypeEnum value as a JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The VehicleTypeEnum value to write</param>
    /// <param name="options">JSON serializer options</param>
    public override void Write(Utf8JsonWriter writer, VehicleTypeEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDisplayString());
    }
}
