using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.FeeStrategies;

namespace BidCalculationTool.Test.Domain.FeeStrategies;

public class SpecialFeeStrategyTest
{
    private readonly SpecialFeeStrategy _strategy;

    public SpecialFeeStrategyTest()
    {
        _strategy = new SpecialFeeStrategy();
    }

    [Fact]
    public void FeeName_ShouldReturnSpecialFee()
    {
        // Act
        var feeName = _strategy.FeeName;

        // Assert
        Assert.Equal("SpecialFee", feeName);
    }

    [Fact]
    public void DisplayName_ShouldReturnCorrectDisplayName()
    {
        // Act
        var displayName = _strategy.DisplayName;

        // Assert
        Assert.Equal("Seller's Special Fee", displayName);
    }

    [Fact]
    public void Description_ShouldReturnCorrectDescription()
    {
        // Act
        var description = _strategy.Description;

        // Assert
        Assert.Equal("Percentage-based fee: 2% for Common vehicles, 4% for Luxury vehicles", description);
    }

    [Theory]
    [InlineData(1000.00, VehicleTypeEnum.Common, 20.00)]  // 2% of 1000
    [InlineData(500.00, VehicleTypeEnum.Common, 10.00)]   // 2% of 500
    [InlineData(2500.00, VehicleTypeEnum.Common, 50.00)]  // 2% of 2500
    [InlineData(0.01, VehicleTypeEnum.Common, 0.0002)]    // 2% of 0.01
    public void Calculate_CommonVehicle_ShouldCalculate2Percent(decimal vehiclePrice, VehicleTypeEnum vehicleType, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Theory]
    [InlineData(1000.00, VehicleTypeEnum.Luxury, 40.00)]  // 4% of 1000
    [InlineData(500.00, VehicleTypeEnum.Luxury, 20.00)]   // 4% of 500
    [InlineData(2500.00, VehicleTypeEnum.Luxury, 100.00)] // 4% of 2500
    [InlineData(0.01, VehicleTypeEnum.Luxury, 0.0004)]    // 4% of 0.01
    public void Calculate_LuxuryVehicle_ShouldCalculate4Percent(decimal vehiclePrice, VehicleTypeEnum vehicleType, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Fact]
    public void Calculate_InvalidVehicleType_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidVehicleType = (VehicleTypeEnum)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _strategy.Calculate(1000, invalidVehicleType));
        Assert.Contains("Invalid vehicle type:", exception.Message);
    }

    [Theory]
    [InlineData((VehicleTypeEnum)999)]
    [InlineData((VehicleTypeEnum)(-1))]
    [InlineData((VehicleTypeEnum)100)]
    public void Calculate_MultipleInvalidVehicleTypes_ShouldThrowArgumentException(VehicleTypeEnum invalidVehicleType)
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
        Assert.Equal("SpecialFee", _strategy.FeeName);
        Assert.Equal("Seller's Special Fee", _strategy.DisplayName);
        Assert.NotNull(_strategy.Description);
        Assert.Contains("2%", _strategy.Description);
        Assert.Contains("4%", _strategy.Description);
        Assert.Contains("Common", _strategy.Description);
        Assert.Contains("Luxury", _strategy.Description);
    }

    [Theory]
    [InlineData(-100.00)]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    public void Calculate_NegativeOrZeroPrice_ShouldCalculateCorrectly(decimal price)
    {
        // Test edge cases with negative or zero prices to ensure all calculation paths are covered

        // Act
        var commonResult = _strategy.Calculate(price, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(price, VehicleTypeEnum.Luxury);

        // Assert - Should calculate percentage even for negative values
        Assert.Equal(price * 0.02m, commonResult); // 2% of price
        Assert.Equal(price * 0.04m, luxuryResult); // 4% of price
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act - Create instance to force constructor execution
        var strategy = new SpecialFeeStrategy();

        // Assert - Validate all fields are properly initialized
        Assert.NotNull(strategy);
        Assert.Equal("SpecialFee", strategy.FeeName);
        Assert.Equal("Seller's Special Fee", strategy.DisplayName);
        Assert.NotNull(strategy.Description);

        // Validate that constants are accessible through calculations
        var commonResult = strategy.Calculate(100m, VehicleTypeEnum.Common);
        var luxuryResult = strategy.Calculate(100m, VehicleTypeEnum.Luxury);
        Assert.Equal(2.00m, commonResult); // This proves CommonVehicleFeePercentage is initialized
        Assert.Equal(4.00m, luxuryResult); // This proves LuxuryVehicleFeePercentage is initialized
    }

    [Fact]
    public void StaticTypeInfo_ShouldBeAccessible()
    {
        // This test forces static initialization by accessing type information
        var type = typeof(SpecialFeeStrategy);
        Assert.NotNull(type);
        Assert.Equal("SpecialFeeStrategy", type.Name);

        // Create instance to ensure all static elements are loaded
        var instance = Activator.CreateInstance(type) as SpecialFeeStrategy;
        Assert.NotNull(instance);

        // Verify static behavior works
        var commonResult = instance!.Calculate(50m, VehicleTypeEnum.Common);
        var luxuryResult = instance.Calculate(50m, VehicleTypeEnum.Luxury);
        Assert.Equal(1.00m, commonResult); // 2% of 50
        Assert.Equal(2.00m, luxuryResult); // 4% of 50
    }

    [Fact]
    public void ForceStaticInitialization_ShouldTriggerCctor()
    {
        // Force static constructor execution using RuntimeHelpers
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(SpecialFeeStrategy).TypeHandle);

        // Verify class works after forced initialization
        var strategy = new SpecialFeeStrategy();
        var commonResult = strategy.Calculate(25m, VehicleTypeEnum.Common);
        var luxuryResult = strategy.Calculate(25m, VehicleTypeEnum.Luxury);
        Assert.Equal(0.50m, commonResult); // 2% of 25
        Assert.Equal(1.00m, luxuryResult); // 4% of 25
    }
}
