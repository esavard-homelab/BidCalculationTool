using System.Text;
using BidCalculationTool.Api.Converters;
using BidCalculationTool.Domain.Enums;
using System.Text.Json;

namespace BidCalculationTool.Test.Api;

public class VehicleTypeEnumJsonConverterTest
{
    private readonly VehicleTypeEnumJsonConverter _converter;
    private readonly JsonSerializerOptions _options;

    public VehicleTypeEnumJsonConverterTest()
    {
        _converter = new VehicleTypeEnumJsonConverter();
        _options = new JsonSerializerOptions();
        _options.Converters.Add(_converter);
    }

    [Theory]
    [InlineData("\"COMMON\"", VehicleTypeEnum.Common)]
    [InlineData("\"LUXURY\"", VehicleTypeEnum.Luxury)]
    [InlineData("\"common\"", VehicleTypeEnum.Common)]
    [InlineData("\"luxury\"", VehicleTypeEnum.Luxury)]
    [InlineData("\"Common\"", VehicleTypeEnum.Common)]
    [InlineData("\"Luxury\"", VehicleTypeEnum.Luxury)]
    public void Read_WithValidStringValues_ShouldReturnCorrectEnum(string json, VehicleTypeEnum expected)
    {
        // Act
        var result = JsonSerializer.Deserialize<VehicleTypeEnum>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123")]           // Number instead of string
    [InlineData("true")]          // Boolean instead of string
    [InlineData("null")]          // Null value
    [InlineData("{}")]            // Object instead of string
    [InlineData("[]")]            // Array instead of string
    public void Read_WithNonStringTokenType_ShouldThrowJsonException(string json)
    {
        // Act & Assert
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<VehicleTypeEnum>(json, _options));

        Assert.Contains("Expected string value for VehicleTypeEnum", exception.Message);
    }

    [Theory]
    [InlineData("\"\"")]          // Empty string
    [InlineData("\"   \"")]       // Whitespace only
    [InlineData("\"\\t\"")]       // Tab character
    [InlineData("\"\\n\"")]       // Newline character
    public void Read_WithNullOrWhitespaceString_ShouldThrowJsonException(string json)
    {
        // Act & Assert
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<VehicleTypeEnum>(json, _options));

        Assert.Equal("Vehicle type cannot be null or empty", exception.Message);
    }

    [Theory]
    [InlineData("\"INVALID\"")]
    [InlineData("\"CAR\"")]
    [InlineData("\"TRUCK\"")]
    [InlineData("\"xyz\"")]
    [InlineData("\"123\"")]
    public void Read_WithInvalidVehicleType_ShouldThrowJsonException(string json)
    {
        // Act & Assert
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<VehicleTypeEnum>(json, _options));

        Assert.Contains("Invalid vehicle type:", exception.Message);
    }

    [Theory]
    [InlineData(VehicleTypeEnum.Common, "Common")]
    [InlineData(VehicleTypeEnum.Luxury, "Luxury")]
    public void Write_WithValidEnum_ShouldWriteCorrectString(VehicleTypeEnum value, string expected)
    {
        // Act
        var json = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal($"\"{expected}\"", json);
    }

    [Fact]
    public void Read_WithDirectConverterCall_ShouldHandleAllTokenTypes()
    {
        // Arrange
        const string json = "42";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act & Assert
        JsonException? exception = null;
        try
        {
            _converter.Read(ref reader, typeToConvert: typeof(VehicleTypeEnum), options: _options);
        }
        catch (JsonException ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception);
        Assert.Contains("Expected string value for VehicleTypeEnum, got Number", exception.Message);
    }

    [Fact]
    public void Read_WithNullJsonString_ShouldThrowJsonException()
    {
        // Arrange
        const string json = "null";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act & Assert
        JsonException? exception = null;
        try
        {
            _converter.Read(ref reader, typeof(VehicleTypeEnum), _options);
        }
        catch (JsonException ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception);
        Assert.Contains("Expected string value for VehicleTypeEnum, got Null", exception.Message);
    }

    [Fact]
    public void Write_WithInvalidEnum_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const VehicleTypeEnum invalidEnum = (VehicleTypeEnum)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            JsonSerializer.Serialize(invalidEnum, _options));

        Assert.Equal("vehicleTypeEnum", exception.ParamName);
    }
}
