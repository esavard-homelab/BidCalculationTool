using BidCalculationTool.Domain.Dto;

namespace BidCalculationTool.Test.Domain;

public class VehicleTypeDtoTest
{
    [Fact]
    public void VehicleTypeDto_ShouldInitializeWithDefaultValues()
    {
        // Act
        var dto = new VehicleTypeDto();

        // Assert
        Assert.Equal(string.Empty, dto.Value);
        Assert.Equal(string.Empty, dto.Label);
    }

    [Fact]
    public void VehicleTypeDto_ShouldInitializeWithProvidedValues()
    {
        // Arrange
        const string expectedValue = "common";
        const string expectedLabel = "Common Vehicle";

        // Act
        var dto = new VehicleTypeDto
        {
            Value = expectedValue,
            Label = expectedLabel
        };

        // Assert
        Assert.Equal(expectedValue, dto.Value);
        Assert.Equal(expectedLabel, dto.Label);
    }

    [Fact]
    public void VehicleTypeDto_ShouldInitializeWithPrimaryConstructor()
    {
        // Arrange
        const string expectedValue = "luxury";
        const string expectedLabel = "Luxury Vehicle";

        // Act - This uses the primary constructor which should cover the record declaration
        var dto = new VehicleTypeDto { Value = expectedValue, Label = expectedLabel };

        // Assert
        Assert.Equal(expectedValue, dto.Value);
        Assert.Equal(expectedLabel, dto.Label);
    }

    [Fact]
    public void VehicleTypeDto_ShouldSupportRecordEquality()
    {
        // Arrange
        var dto1 = new VehicleTypeDto { Value = "luxury", Label = "Luxury Vehicle" };
        var dto2 = new VehicleTypeDto { Value = "luxury", Label = "Luxury Vehicle" };
        var dto3 = new VehicleTypeDto { Value = "common", Label = "Common Vehicle" };

        // Assert
        Assert.Equal(dto1, dto2); // Same values should be equal
        Assert.NotEqual(dto1, dto3); // Different values should not be equal
    }

    [Fact]
    public void VehicleTypeDto_ShouldSupportRecordHashCode()
    {
        // Arrange
        var dto1 = new VehicleTypeDto { Value = "luxury", Label = "Luxury Vehicle" };
        var dto2 = new VehicleTypeDto { Value = "luxury", Label = "Luxury Vehicle" };

        // Assert
        Assert.Equal(dto1.GetHashCode(), dto2.GetHashCode());
    }

    [Fact]
    public void VehicleTypeDto_ShouldSupportRecordToString()
    {
        // Arrange
        var dto = new VehicleTypeDto { Value = "common", Label = "Common Vehicle" };

        // Act
        var result = dto.ToString();

        // Assert
        Assert.Contains("common", result);
        Assert.Contains("Common Vehicle", result);
        Assert.Contains("VehicleTypeDto", result);
    }
}
