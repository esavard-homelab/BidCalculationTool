using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.FeeStrategies;

namespace BidCalculationTool.Test.Domain.FeeStrategies;

public class FixedStorageFeeStrategyTest
{
    private readonly FixedStorageFeeStrategy _strategy;

    public FixedStorageFeeStrategyTest()
    {
        _strategy = new FixedStorageFeeStrategy();
    }

    [Fact]
    public void FeeName_ShouldReturnStorageFee()
    {
        // Act
        var feeName = _strategy.FeeName;

        // Assert
        Assert.Equal("StorageFee", feeName);
    }

    [Fact]
    public void DisplayName_ShouldReturnCorrectDisplayName()
    {
        // Act
        var displayName = _strategy.DisplayName;

        // Assert
        Assert.Equal("Storage Fee", displayName);
    }

    [Fact]
    public void Description_ShouldReturnCorrectDescription()
    {
        // Act
        var description = _strategy.Description;

        // Assert
        Assert.Equal("Fixed storage fee applied to all vehicle auctions", description);
    }

    [Theory]
    [InlineData(1.00, VehicleTypeEnum.Common, 100.00)]
    [InlineData(1000.00, VehicleTypeEnum.Common, 100.00)]
    [InlineData(1000000.00, VehicleTypeEnum.Common, 100.00)]
    [InlineData(1.00, VehicleTypeEnum.Luxury, 100.00)]
    [InlineData(1000.00, VehicleTypeEnum.Luxury, 100.00)]
    [InlineData(1000000.00, VehicleTypeEnum.Luxury, 100.00)]
    public void Calculate_AnyPriceAndType_ShouldAlwaysReturn100(decimal vehiclePrice, VehicleTypeEnum vehicleType, decimal expectedFee)
    {
        // Act
        var result = _strategy.Calculate(vehiclePrice, vehicleType);

        // Assert
        Assert.Equal(expectedFee, result);
    }

    [Fact]
    public void Calculate_IsAlwaysConstant_ShouldReturn100()
    {
        // Arrange
        var testCases = new[]
        {
            (0.01m, VehicleTypeEnum.Common),
            (50.00m, VehicleTypeEnum.Luxury),
            (999999.99m, VehicleTypeEnum.Common),
            (decimal.MaxValue, VehicleTypeEnum.Luxury)
        };

        // Act & Assert
        foreach (var (price, vehicleType) in testCases)
        {
            var result = _strategy.Calculate(price, vehicleType);
            Assert.Equal(100.00m, result);
        }
    }

    [Theory]
    [InlineData(-100.00)]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    public void Calculate_NegativeOrZeroPrice_ShouldStillReturnFixedAmount(decimal price)
    {
        // Test edge cases - fixed fee should always be returned regardless of price

        // Act
        var commonResult = _strategy.Calculate(price, VehicleTypeEnum.Common);
        var luxuryResult = _strategy.Calculate(price, VehicleTypeEnum.Luxury);

        // Assert - Should always return fixed amount
        Assert.Equal(100.00m, commonResult);
        Assert.Equal(100.00m, luxuryResult);
    }

    [Theory]
    [InlineData((VehicleTypeEnum)999)]
    [InlineData((VehicleTypeEnum)(-1))]
    [InlineData((VehicleTypeEnum)100)]
    public void Calculate_InvalidVehicleTypes_ShouldStillReturnFixedAmount(VehicleTypeEnum invalidVehicleType)
    {
        // Fixed storage fee should be returned regardless of vehicle type validity
        // since it doesn't depend on vehicle type

        // Act
        var result = _strategy.Calculate(1000m, invalidVehicleType);

        // Assert
        Assert.Equal(100.00m, result);
    }

    [Fact]
    public void AllProperties_ShouldReturnExpectedValues()
    {
        // Comprehensive test to ensure all properties are accessed and covered

        // Act & Assert
        Assert.Equal("StorageFee", _strategy.FeeName);
        Assert.Equal("Storage Fee", _strategy.DisplayName);
        Assert.NotNull(_strategy.Description);
        Assert.Contains("Fixed storage fee", _strategy.Description);
        Assert.Contains("applied to all vehicle auctions", _strategy.Description);
    }

    [Fact]
    public void Calculate_MultipleInvocations_ShouldReturnConsistentValue()
    {
        // Test that the fixed storage amount constant is consistently returned

        // Act
        var results = new[]
        {
            _strategy.Calculate(100m, VehicleTypeEnum.Common),
            _strategy.Calculate(200m, VehicleTypeEnum.Luxury),
            _strategy.Calculate(300m, VehicleTypeEnum.Common),
            _strategy.Calculate(-50m, VehicleTypeEnum.Luxury),
            _strategy.Calculate(0m, VehicleTypeEnum.Common)
        };

        // Assert - All results should be identical
        Assert.All(results, result => Assert.Equal(100.00m, result));
        Assert.True(results.All(r => r == 100.00m));
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act - Create instance to force constructor execution
        var strategy = new FixedStorageFeeStrategy();

        // Assert - Validate all fields are properly initialized
        Assert.NotNull(strategy);
        Assert.Equal("StorageFee", strategy.FeeName);
        Assert.Equal("Storage Fee", strategy.DisplayName);
        Assert.NotNull(strategy.Description);

        // Validate that the constant is accessible through calculation
        var result = strategy.Calculate(1000m, VehicleTypeEnum.Common);
        Assert.Equal(100.00m, result); // This proves FixedStorageAmount is initialized
    }

    [Fact]
    public void StaticTypeInfo_ShouldBeAccessible()
    {
        // This test forces static initialization by accessing type information
        var type = typeof(FixedStorageFeeStrategy);
        Assert.NotNull(type);
        Assert.Equal("FixedStorageFeeStrategy", type.Name);

        // Create instance to ensure all static elements are loaded
        var instance = Activator.CreateInstance(type) as FixedStorageFeeStrategy;
        Assert.NotNull(instance);

        // Verify static behavior works
        var result = instance!.Calculate(500m, VehicleTypeEnum.Luxury);
        Assert.Equal(100.00m, result); // Fixed amount
    }

    [Fact]
    public void ForceStaticInitialization_ShouldTriggerCctor()
    {
        // Force static constructor execution using RuntimeHelpers
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(FixedStorageFeeStrategy).TypeHandle);

        // Verify class works after forced initialization
        var strategy = new FixedStorageFeeStrategy();
        var result = strategy.Calculate(999m, VehicleTypeEnum.Common);
        Assert.Equal(100.00m, result); // Should return fixed amount
    }
}
