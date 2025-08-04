using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.FeeStrategies;

namespace BidCalculationTool.Test.Domain.FeeStrategies;

public class BasicBuyerFeeStrategyTest
{
    private readonly BasicBuyerFeeStrategy _strategy = new();

    [Fact]
    public void FeeName_ShouldReturnBasicBuyerFee()
    {
        // Act
        var feeName = _strategy.FeeName;

        // Assert
        Assert.Equal("BasicBuyerFee", feeName);
    }

    [Fact]
    public void DisplayName_ShouldReturnCorrectDisplayName()
    {
        // Act
        var displayName = _strategy.DisplayName;

        // Assert
        Assert.Equal("Basic Buyer Fee", displayName);
    }

    [Fact]
    public void Description_ShouldReturnCorrectDescription()
    {
        // Act
        var description = _strategy.Description;

        // Assert
        Assert.Equal("10% of vehicle price with minimum and maximum limits based on vehicle type", description);
    }

    [Theory]
    [InlineData(100.00, VehicleTypeEnum.Common, 10.00)]  // 10% of 100 = 10, min is 10
    [InlineData(300.00, VehicleTypeEnum.Common, 30.00)]  // 10% of 300 = 30
    [InlineData(500.00, VehicleTypeEnum.Common, 50.00)]  // 10% of 500 = 50, max is 50
    [InlineData(1000.00, VehicleTypeEnum.Common, 50.00)] // 10% of 1000 = 100, capped at 50
    [InlineData(50.00, VehicleTypeEnum.Common, 10.00)]   // 10% of 50 = 5, min is 10
    public void Calculate_CommonVehicle_ShouldApplyCorrectLimits(decimal vehiclePrice, VehicleTypeEnum vehicleType, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(100.00, VehicleTypeEnum.Luxury, 25.00)]   // 10% of 100 = 10, min is 25
    [InlineData(300.00, VehicleTypeEnum.Luxury, 30.00)]   // 10% of 300 = 30
    [InlineData(2000.00, VehicleTypeEnum.Luxury, 200.00)] // 10% of 2000 = 200, max is 200
    [InlineData(5000.00, VehicleTypeEnum.Luxury, 200.00)] // 10% of 5000 = 500, capped at 200
    [InlineData(200.00, VehicleTypeEnum.Luxury, 25.00)]   // 10% of 200 = 20, min is 25
    public void Calculate_LuxuryVehicle_ShouldApplyCorrectLimits(decimal vehiclePrice, VehicleTypeEnum vehicleType,
        decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Fact]
    public void Calculate_CommonVehicle_ExactMinimumBoundary_ShouldReturnMinimumFee()
    {
        // Arrange - Test exact boundary where 10% equals minimum fee
        const decimal priceWhereCalculatedFeeEqualsMin = 100.00m; // 10% of 100 = 10 (minimum)

        // Act
        var result = _strategy.Calculate(priceWhereCalculatedFeeEqualsMin, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(10.00m, result); // CommonMinimumFee constant
    }

    [Fact]
    public void Calculate_CommonVehicle_ExactMaximumBoundary_ShouldReturnMaximumFee()
    {
        // Arrange - Test exact boundary where 10% equals maximum fee
        const decimal priceWhereCalculatedFeeEqualsMax = 500.00m; // 10% of 500 = 50 (maximum)

        // Act
        var result = _strategy.Calculate(priceWhereCalculatedFeeEqualsMax, VehicleTypeEnum.Common);

        // Assert
        Assert.Equal(50.00m, result); // CommonMaximumFee constant
    }

    [Fact]
    public void Calculate_LuxuryVehicle_ExactMinimumBoundary_ShouldReturnMinimumFee()
    {
        // Arrange - Test exact boundary where 10% equals minimum fee
        const decimal priceWhereCalculatedFeeEqualsMin = 250.00m; // 10% of 250 = 25 (minimum)

        // Act
        var result = _strategy.Calculate(priceWhereCalculatedFeeEqualsMin, VehicleTypeEnum.Luxury);

        // Assert
        Assert.Equal(25.00m, result); // LuxuryMinimumFee constant
    }

    [Fact]
    public void Calculate_LuxuryVehicle_ExactMaximumBoundary_ShouldReturnMaximumFee()
    {
        // Arrange - Test exact boundary where 10% equals maximum fee
        const decimal priceWhereCalculatedFeeEqualsMax = 2000.00m; // 10% of 2000 = 200 (maximum)

        // Act
        var result = _strategy.Calculate(priceWhereCalculatedFeeEqualsMax, VehicleTypeEnum.Luxury);

        // Assert
        Assert.Equal(200.00m, result); // LuxuryMaximumFee constant
    }

    [Fact]
    public void Calculate_FeePercentageValidation_ShouldCalculateExact10Percent()
    {
        // Arrange - Test that FeePercentage constant (10%) is used correctly
        const decimal testPrice = 1337.50m; // Unusual price to test exact percentage
        const decimal expectedCalculatedCappedFeeCommon = 50.00m; // capped at CommonMaximumFee
        const decimal expectedCalculatedFeeLuxury = 133.75m; // 10% of 1337.50

        // Act
        var commonResult = _strategy.Calculate(testPrice, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(testPrice, VehicleTypeEnum.Luxury);

        // Assert - Both should be capped but verify 10% calculation works
        Assert.Equal(expectedCalculatedCappedFeeCommon, commonResult); // Capped at CommonMaximumFee
        Assert.Equal(expectedCalculatedFeeLuxury, luxuryResult); // Within Luxury limits, shows exact 10%
    }

    [Theory]
    [InlineData(0.01)] // Very small price
    [InlineData(1.00)] // $1 price
    public void Calculate_VerySmallPrices_ShouldApplyMinimumFees(decimal verySmallPrice)
    {
        // Act
        var commonResult = _strategy.Calculate(verySmallPrice, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(verySmallPrice, VehicleTypeEnum.Luxury);

        // Assert - Should always apply minimum fees for very small prices
        Assert.Equal(10.00m, commonResult); // CommonMinimumFee
        Assert.Equal(25.00m, luxuryResult); // LuxuryMinimumFee
    }

    [Fact]
    public void Calculate_InvalidVehicleType_ShouldThrowArgumentException()
    {
        // Arrange
        const VehicleTypeEnum invalidVehicleType = (VehicleTypeEnum)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(1000, invalidVehicleType));
        Assert.Contains("Invalid vehicle type:", exception.Message);
    }

    [Theory]
    [InlineData(-100.00)]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    public void Calculate_NegativeOrZeroPrice_ShouldCalculateCorrectly(decimal price)
    {
        // Even with negative or zero prices, the strategy should calculate based on percentage
        // This tests edge cases and ensures all code paths are covered

        // Act
        var commonResult = _strategy.Calculate(price, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(price, VehicleTypeEnum.Luxury);

        // Assert - Negative prices should still apply minimum fees
        Assert.Equal(10.00m, commonResult); // CommonMinimumFee
        Assert.Equal(25.00m, luxuryResult); // LuxuryMinimumFee
    }

    [Fact]
    public void Calculate_ExactPercentageCalculation_ShouldBeAccurate()
    {
        // Test to ensure the 10% calculation is exact and covers the FeePercentage constant
        const decimal testPrice = 123.45m;
        const decimal expectedCalculatedFee = 12.345m; // 10% of 123.45

        // Act
        var commonResult = _strategy.Calculate(testPrice, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(testPrice, VehicleTypeEnum.Luxury);

        // Assert
        Assert.Equal(expectedCalculatedFee, commonResult); // Within Common limits
        Assert.Equal(25.00m, luxuryResult); // Capped at LuxuryMinimumFee
    }

    [Theory]
    [InlineData((VehicleTypeEnum)999)]
    [InlineData((VehicleTypeEnum)(-1))]
    [InlineData((VehicleTypeEnum)100)]
    public void Calculate_InvalidVehicleTypes_ShouldThrowArgumentException(VehicleTypeEnum invalidVehicleType)
    {
        // Test multiple invalid vehicle types to ensure complete branch coverage

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(1000m, invalidVehicleType));
        Assert.Contains("Invalid vehicle type:", exception.Message);
        Assert.Contains(invalidVehicleType.ToString(), exception.Message);
    }

    [Fact]
    public void AllProperties_ShouldReturnExpectedValues()
    {
        // Comprehensive test to ensure all properties are accessed and covered

        // Act & Assert
        Assert.Equal("BasicBuyerFee", _strategy.FeeName);
        Assert.Equal("Basic Buyer Fee", _strategy.DisplayName);
        Assert.NotNull(_strategy.Description);
        Assert.Contains("10%", _strategy.Description);
        Assert.Contains("minimum and maximum limits", _strategy.Description);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act - Create instance to force constructor execution
        var strategy = new BasicBuyerFeeStrategy();

        // Assert - Validate all fields are properly initialized
        Assert.NotNull(strategy);
        Assert.Equal("BasicBuyerFee", strategy.FeeName);
        Assert.Equal("Basic Buyer Fee", strategy.DisplayName);
        Assert.NotNull(strategy.Description);

        // Validate that constants are accessible through calculations
        var result = strategy.Calculate(100m, VehicleTypeEnum.Common);
        Assert.Equal(10.00m, result); // This proves FeePercentage and CommonMinimumFee are initialized
    }

    [Fact]
    public void StaticTypeInfo_ShouldBeAccessible()
    {
        // This test forces static initialization by accessing type information
        var type = typeof(BasicBuyerFeeStrategy);
        Assert.NotNull(type);
        Assert.Equal("BasicBuyerFeeStrategy", type.Name);

        // Create instance to ensure all static elements are loaded
        var instance = Activator.CreateInstance(type) as BasicBuyerFeeStrategy;
        Assert.NotNull(instance);

        // Verify static behavior works
        var result = instance!.Calculate(50m, VehicleTypeEnum.Common);
        Assert.Equal(10.00m, result); // Minimum fee
    }

    [Fact]
    public void ForceStaticInitialization_ShouldTriggerCctor()
    {
        // Force static constructor execution using RuntimeHelpers
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(BasicBuyerFeeStrategy).TypeHandle);

        // Verify class works after forced initialization
        var strategy = new BasicBuyerFeeStrategy();
        var result = strategy.Calculate(75m, VehicleTypeEnum.Common);
        Assert.Equal(10.00m, result); // Should use CommonMinimumFee
    }
}
