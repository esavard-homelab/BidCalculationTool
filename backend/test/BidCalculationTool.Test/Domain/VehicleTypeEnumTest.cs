using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Test.Domain;

public class VehicleTypeEnumTest
{
    [Theory]
    [InlineData(VehicleTypeEnum.Common, "Common")]
    [InlineData(VehicleTypeEnum.Luxury, "Luxury")]
    public void ToDisplayString_WithValidVehicleType_ShouldReturnCorrectDisplayString(
        VehicleTypeEnum vehicleType, string expectedDisplayString)
    {
        // Act
        var result = vehicleType.ToDisplayString();

        // Assert
        Assert.Equal(expectedDisplayString, result);
    }

    [Fact]
    public void ToDisplayString_WithInvalidVehicleType_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidVehicleType = (VehicleTypeEnum)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => invalidVehicleType.ToDisplayString());
        Assert.Equal("vehicleTypeEnum", exception.ParamName);
        Assert.Equal(invalidVehicleType, exception.ActualValue);
    }

    [Fact]
    public void VehicleTypeEnum_ShouldHaveExpectedValues()
    {
        // Assert - Verify that the enum has the expected values
        Assert.True(Enum.IsDefined(typeof(VehicleTypeEnum), VehicleTypeEnum.Common));
        Assert.True(Enum.IsDefined(typeof(VehicleTypeEnum), VehicleTypeEnum.Luxury));
        Assert.Equal(2, Enum.GetValues<VehicleTypeEnum>().Length);
    }

    [Theory]
    [InlineData("COMMON", VehicleTypeEnum.Common)]
    [InlineData("LUXURY", VehicleTypeEnum.Luxury)]
    [InlineData("common", VehicleTypeEnum.Common)]
    [InlineData("luxury", VehicleTypeEnum.Luxury)]
    [InlineData("Common", VehicleTypeEnum.Common)]
    [InlineData("Luxury", VehicleTypeEnum.Luxury)]
    public void FromString_WithValidString_ShouldReturnCorrectEnum(string input, VehicleTypeEnum expected)
    {
        // Act
        var result = VehicleTypeExtensions.FromString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FromString_WithNullOrEmptyString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => VehicleTypeExtensions.FromString(input));
        Assert.Equal("Vehicle type string cannot be null or empty. (Parameter 'vehicleTypeString')", exception.Message);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("CAR")]
    [InlineData("TRUCK")]
    [InlineData("123")]
    public void FromString_WithInvalidString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => VehicleTypeExtensions.FromString(input));
        Assert.Contains($"Invalid vehicle type: '{input}'", exception.Message);
        Assert.Contains("Valid values are: 'Common', 'Luxury', 'COMMON', 'LUXURY'", exception.Message);
    }
}
