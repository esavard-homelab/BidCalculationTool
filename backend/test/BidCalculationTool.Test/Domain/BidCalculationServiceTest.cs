using BidCalculationTool.Domain.Services;
using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Test.Domain;

public class BidCalculationServiceTest
{
    private readonly BidCalculationService _service;

    public BidCalculationServiceTest()
    {
        _service = new BidCalculationService();
    }

    [Theory]
    [InlineData(398.00, VehicleTypeEnum.Common, 39.80, 7.96, 5.00, 100.00, 550.76)]
    [InlineData(501.00, VehicleTypeEnum.Common, 50.00, 10.02, 10.00, 100.00, 671.02)]
    [InlineData(57.00, VehicleTypeEnum.Common, 10.00, 1.14, 5.00, 100.00, 173.14)]
    [InlineData(1800.00, VehicleTypeEnum.Luxury, 180.00, 72.00, 15.00, 100.00, 2167.00)]
    [InlineData(1100.00, VehicleTypeEnum.Common, 50.00, 22.00, 15.00, 100.00, 1287.00)]
    [InlineData(1000000.00, VehicleTypeEnum.Luxury, 200.00, 40000.00, 20.00, 100.00, 1040320.00)]
    public void CalculateTotalPrice_ShouldReturnCorrectFeesAndTotalPrice_ForGivenVehiclePriceAndType(
        decimal vehiclePrice,
        VehicleTypeEnum vehicleType,
        decimal expectedBasicFee,
        decimal expectedSpecialFee,
        decimal expectedAssociationFee,
        decimal expectedStorageFee,
        decimal expectedTotal)
    {
        // Arrange
        var service = new BidCalculationService();
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = vehiclePrice,
            VehicleType = vehicleType
        };
        var expectedResponse = new BidCalculationResponseDto
        {
            VehiclePrice = vehiclePrice,
            VehicleType = vehicleType,
            BasicBuyerFee = expectedBasicFee,
            SellerSpecialFee = expectedSpecialFee,
            AssociationFee = expectedAssociationFee,
            StorageFee = expectedStorageFee,
            TotalCost = expectedTotal
        };

        // Act
        var actualResponse = service.CalculateTotalPrice(
            request);

        // Assert
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public void CalculateBasicBuyerFee_Common_ShouldApplyMinAndMaxLimits()
    {
        // Arrange
        const decimal minFee = 10.00m;
        const decimal maxFee = 50.00m;

        var requestMin = new BidCalculationRequestDto
        {
            VehiclePrice = 50.00m,
            VehicleType = VehicleTypeEnum.Common
        };

        var requestMax = new BidCalculationRequestDto
        {
            VehiclePrice = 1000.00m,
            VehicleType = VehicleTypeEnum.Common
        };
        var service = new BidCalculationService();

        // Act
        var actualMinBasicBuyerFee = service.CalculateTotalPrice(requestMin);
        var actualMaxBasicBuyerFee = service.CalculateTotalPrice(requestMax);

        // Assert
        Assert.Equal(minFee, actualMinBasicBuyerFee.BasicBuyerFee);
        Assert.Equal(maxFee, actualMaxBasicBuyerFee.BasicBuyerFee);
    }

    [Fact]
    public void CalculateBasicBuyerFee_Luxury_ShouldApplyMinAndMaxLimits()
    {
        // Arrange
        const decimal minFee = 25.00m;
        const decimal maxFee = 200.00m;

        var requestMin = new BidCalculationRequestDto
        {
            VehiclePrice = 100.00m,
            VehicleType = VehicleTypeEnum.Luxury
        };

        var requestMax = new BidCalculationRequestDto
        {
            VehiclePrice = 5000.00m,
            VehicleType = VehicleTypeEnum.Luxury
        };
        var service = new BidCalculationService();

        // Act
        var actualMinBasicBuyerFee = service.CalculateTotalPrice(requestMin);
        var actualMaxBasicBuyerFee = service.CalculateTotalPrice(requestMax);

        // Assert
        Assert.Equal(minFee, actualMinBasicBuyerFee.BasicBuyerFee);
        Assert.Equal(maxFee, actualMaxBasicBuyerFee.BasicBuyerFee);
    }

    [Theory]
    [InlineData(250, 5.00)]   // $1-$500
    [InlineData(750, 10.00)]  // $501-$1000
    [InlineData(2000, 15.00)] // $1001-$3000
    [InlineData(5000, 20.00)] // $3001+
    public void CalculateAssociationFee_ShouldReturnCorrectFee_BasedOnPriceRange(
        decimal price, decimal expected)
    {
        // Arrange
        var service = new BidCalculationService();
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = price,
            VehicleType = VehicleTypeEnum.Common // Association fee is the same for Common and Luxury
        };
        var expectedResponse = new BidCalculationResponseDto
        {
            VehiclePrice = price,
            VehicleType = VehicleTypeEnum.Common,
            AssociationFee = expected,
            BasicBuyerFee = 0,
            SellerSpecialFee = 0,
            StorageFee = 0,
            TotalCost = 0
        };

        // Act
        var response = service.CalculateTotalPrice(request);

        // Assert
        Assert.Equal(expectedResponse.AssociationFee, response.AssociationFee);
    }

    [Fact]
    public void CalculateTotalPrice_WithCommonVehicle_ShouldReturnCorrectValues()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 1000m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Act
        var result = _service.CalculateTotalPrice(request);

        // Assert - Access all properties to cover all getters
        Assert.NotNull(result);
        Assert.Equal(1000m, result.VehiclePrice);
        Assert.Equal(VehicleTypeEnum.Common, result.VehicleType);
        Assert.Equal(50m, result.BasicBuyerFee); // 10% of 1000, capped at 50
        Assert.Equal(20m, result.SellerSpecialFee); // 2% of 1000
        Assert.Equal(10m, result.AssociationFee); // 1000 is in 500-1000 range
        Assert.Equal(100m, result.StorageFee); // Fixed fee
        Assert.Equal(1180m, result.TotalCost); // 1000 + 50 + 20 + 10 + 100
    }

    [Fact]
    public void CalculateTotalPrice_WithLuxuryVehicle_ShouldReturnCorrectValues()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 2000m,
            VehicleType = VehicleTypeEnum.Luxury
        };

        // Act
        var result = _service.CalculateTotalPrice(request);

        // Assert - Access all properties to cover all getters
        Assert.NotNull(result);
        Assert.Equal(2000m, result.VehiclePrice);
        Assert.Equal(VehicleTypeEnum.Luxury, result.VehicleType);
        Assert.Equal(200m, result.BasicBuyerFee); // 10% of 2000, capped at 200
        Assert.Equal(80m, result.SellerSpecialFee); // 4% of 2000
        Assert.Equal(15m, result.AssociationFee); // 2000 is in 1000-3000 range
        Assert.Equal(100m, result.StorageFee); // Fixed fee
        Assert.Equal(2395m, result.TotalCost); // 2000 + 200 + 80 + 15 + 100
    }

    [Fact]
    public void CalculateTotalPrice_WithNegativeVehiclePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = -100m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CalculateTotalPrice(request));
        Assert.Equal("Vehicle price cannot be negative.", exception.Message);
    }

    [Fact]
    public void CalculateTotalPrice_WithZeroVehiclePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 0m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CalculateTotalPrice(request));
        Assert.Equal("Vehicle price cannot be negative.", exception.Message);
    }

    [Fact]
    public void CalculateTotalPrice_WithInvalidVehicleType_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 1000m,
            VehicleType = (VehicleTypeEnum)999 // Invalid enum value
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CalculateTotalPrice(request));
        Assert.Contains("Invalid vehicle type:", exception.Message);
    }

    [Fact]
    public void CalculateTotalPrice_WithInvalidVehicleTypeForSpecialFee_ShouldThrowArgumentException()
    {
        // Arrange - This test specifically targets the CalculateSpecialFee path
        // by using a scenario where we know CalculateBasicBuyerFee would succeed
        // but CalculateSpecialFee would fail
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 100m, // Low price to ensure we hit CalculateSpecialFee
            VehicleType = (VehicleTypeEnum)888 // Different invalid enum value
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CalculateTotalPrice(request));
        Assert.Contains("Invalid vehicle type:", exception.Message);
        Assert.Contains("888", exception.Message);
    }
}
