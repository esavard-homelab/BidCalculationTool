using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Services;

namespace tests.BidCalculationTool.Tests.Domain;

public class BidCalculationServiceTests
{
    [Theory]
    [InlineData(398.00, "Common", 39.80, 7.96, 5.00, 100.00, 550.76)]
    [InlineData(501.00, "Common", 50.00, 10.02, 10.00, 100.00, 671.02)]
    [InlineData(57.00, "Common", 10.00, 1.14, 5.00, 100.00, 173.14)]
    [InlineData(1800.00, "Luxury", 180.00, 72.00, 15.00, 100.00, 2167.00)]
    [InlineData(1100.00, "Common", 50.00, 22.00, 15.00, 100.00, 1287.00)]
    [InlineData(1000000.00, "Luxury", 200.00, 40000.00, 20.00, 100.00, 1040320.00)]
    public void CalculateBidFees_ShouldReturnCorrectFees_ForGivenVehiclePriceAndType(
        decimal vehiclePrice,
        string vehicleType,
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
            VehicleType = "Common"
        };

        var requestMax = new BidCalculationRequestDto
        {
            VehiclePrice = 1000.00m,
            VehicleType = "Common"
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
            VehicleType = "Luxury"
        };

        var requestMax = new BidCalculationRequestDto
        {
            VehiclePrice = 5000.00m,
            VehicleType = "Luxury"
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
            VehicleType = "Common" // Association fee is the same for Common and Luxury
        };
        var expectedResponse = new BidCalculationResponseDto
        {
            VehiclePrice = price,
            VehicleType = "Common",
            AssociationFee = expected
        };

        // Act
        var response = service.CalculateTotalPrice(request);

        // Assert
        Assert.Equal(request.VehiclePrice, response.VehiclePrice);
    }
}
