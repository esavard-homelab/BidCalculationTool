using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.FeeStrategies;

namespace BidCalculationTool.Test.Domain.FeeStrategies;

public class AssociationFeeStrategyTest
{
    private readonly AssociationFeeStrategy _strategy = new();

    [Fact]
    public void StaticInitialization_ShouldBeTriggered()
    {
        // This test ensures that the static constructor (.cctor()) is executed
        // by creating multiple instances and accessing static-dependent functionality

        // Act - Force static initialization by creating instances and using them
        var strategy1 = new AssociationFeeStrategy();
        var strategy2 = new AssociationFeeStrategy();

        // Assert - Verify static initialization worked by testing calculations
        var result1 = strategy1.Calculate(100.00m, VehicleTypeEnum.Common);
        var result2 = strategy2.Calculate(100.00m, VehicleTypeEnum.Luxury);

        Assert.Equal(5.00m, result1);
        Assert.Equal(5.00m, result2);
        Assert.Equal(result1, result2); // Should be identical since it uses static FeeTiers
    }

    [Fact]
    public void FeeName_ShouldReturnAssociationFee()
    {
        // Act
        var feeName = _strategy.FeeName;

        // Assert
        Assert.Equal("AssociationFee", feeName);
    }

    [Fact]
    public void DisplayName_ShouldReturnCorrectDisplayName()
    {
        // Act
        var displayName = _strategy.DisplayName;

        // Assert
        Assert.Equal("Association Fee", displayName);
    }

    [Fact]
    public void Description_ShouldReturnCorrectDescription()
    {
        // Act
        var description = _strategy.Description;

        // Assert
        Assert.Equal("Tiered fee based on vehicle price. Applies to all vehicle types.", description);
    }

    [Theory]
    [InlineData(1.00, 5.00)]     // Tier 1: $1 to $500 - lower bound
    [InlineData(250.00, 5.00)]   // Tier 1: middle of range
    [InlineData(500.00, 5.00)]   // Tier 1: upper bound
    public void Calculate_Tier1Prices_ShouldReturn5Dollars(decimal vehiclePrice, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(500.01, 10.00)]  // Tier 2: $500.01 to $1,000 - lower bound
    [InlineData(750.00, 10.00)]  // Tier 2: middle of range
    [InlineData(1000.00, 10.00)] // Tier 2: upper bound
    public void Calculate_Tier2Prices_ShouldReturn10Dollars(decimal vehiclePrice, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(1000.01, 15.00)] // Tier 3: $1,000.01 to $3,000 - lower bound
    [InlineData(2000.00, 15.00)] // Tier 3: middle of range
    [InlineData(3000.00, 15.00)] // Tier 3: upper bound
    public void Calculate_Tier3Prices_ShouldReturn15Dollars(decimal vehiclePrice, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(3000.01, 20.00)]    // Tier 4: Above $3,000 - lower bound
    [InlineData(5000.00, 20.00)]    // Tier 4: higher price
    [InlineData(1000000.00, 20.00)] // Tier 4: very high price
    public void Calculate_Tier4Prices_ShouldReturn20Dollars(decimal vehiclePrice, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(VehicleTypeEnum.Common)]
    [InlineData(VehicleTypeEnum.Luxury)]
    public void Calculate_SameForAllVehicleTypes_ShouldReturnSameFee(VehicleTypeEnum vehicleType)
    {
        // Arrange
        const decimal testPrice = 1000.00m;
        const decimal expectedFee = 10.00m;

        // Act
        var result = _strategy.Calculate(testPrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(-100.00)]
    [InlineData(0.00)]
    [InlineData(-0.01)]
    public void Calculate_NegativeOrZeroPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(invalidPrice, VehicleTypeEnum.Common));
        Assert.Contains("Vehicle price cannot be negative or zero", exception.Message);
        Assert.Equal("vehiclePrice", exception.ParamName);
    }

    [Theory]
    [InlineData(10000000000000000000)] // Not a valid price, should throw exception
    public void Calculate_InvalidPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(invalidPrice, VehicleTypeEnum.Common));
        Assert.Contains("No fee tier found for vehicle price", exception.Message);
        Assert.Equal("vehiclePrice", exception.ParamName);
    }

    [Theory]
    [InlineData(10000000.01)] // Price above maximum tier range
    public void Calculate_PriceAboveMaximumTier_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(invalidPrice, VehicleTypeEnum.Common));
        Assert.Contains("No fee tier found for vehicle price", exception.Message);
        Assert.Equal("vehiclePrice", exception.ParamName);
    }

    [Fact]
    public void MultipleInstances_ShouldShareStaticConfiguration()
    {
        // This test verifies that the static FeeTiers array is properly shared
        // between multiple instances and ensures comprehensive static initialization coverage

        // Arrange
        var strategies = new[]
        {
            new AssociationFeeStrategy(),
            new AssociationFeeStrategy(),
            new AssociationFeeStrategy()
        };

        const decimal testPrice = 1500.00m; // Should be in tier 3
        const decimal expectedFee = 15.00m;

        // Act & Assert
        foreach (var strategy in strategies)
        {
            var result = strategy.Calculate(testPrice, VehicleTypeEnum.Common);
            Assert.Equal(expectedFee, result);
        }
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act - Create instance to force constructor execution
        var strategy = new AssociationFeeStrategy();

        // Assert - Validate all fields are properly initialized
        Assert.NotNull(strategy);
        Assert.Equal("AssociationFee", strategy.FeeName);
        Assert.Equal("Association Fee", strategy.DisplayName);
        Assert.NotNull(strategy.Description);

        // Validate that FeeTiers static array is accessible through calculations
        var result = strategy.Calculate(100m, VehicleTypeEnum.Common);
        Assert.Equal(5.00m, result); // This proves FeeTiers static array is initialized
    }
}
