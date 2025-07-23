using BidCalculationTool.Api.Controllers;
using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BidCalculationTool.Test.Api;

public class BidCalculationControllerTest
{
    private readonly Mock<IBidCalculationService> _mockBidCalculationService;
    private readonly BidCalculationController _controller;

    public BidCalculationControllerTest()
    {
        _mockBidCalculationService = new Mock<IBidCalculationService>();
        _controller = new BidCalculationController(_mockBidCalculationService.Object);
    }

    [Fact]
    public void Constructor_WithValidService_ShouldCreateController()
    {
        // Arrange
        var mockService = new Mock<IBidCalculationService>();

        // Act
        var controller = new BidCalculationController(mockService.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void CalculateBid_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 1000m,
            VehicleType = VehicleTypeEnum.Common
        };

        var expectedResponse = new BidCalculationResponseDto
        {
            VehiclePrice = 1000m,
            VehicleType = VehicleTypeEnum.Common,
            BasicBuyerFee = 50m,
            SellerSpecialFee = 20m,
            AssociationFee = 10m,
            StorageFee = 100m,
            TotalCost = 1180m
        };

        _mockBidCalculationService
            .Setup(s => s.CalculateTotalPrice(request))
            .Returns(expectedResponse);

        // Act
        var result = _controller.CalculateBid(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualResponse = Assert.IsType<BidCalculationResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse.VehiclePrice, actualResponse.VehiclePrice);
        Assert.Equal(expectedResponse.VehicleType, actualResponse.VehicleType);
        Assert.Equal(expectedResponse.BasicBuyerFee, actualResponse.BasicBuyerFee);
        Assert.Equal(expectedResponse.SellerSpecialFee, actualResponse.SellerSpecialFee);
        Assert.Equal(expectedResponse.AssociationFee, actualResponse.AssociationFee);
        Assert.Equal(expectedResponse.StorageFee, actualResponse.StorageFee);
        Assert.Equal(expectedResponse.TotalCost, actualResponse.TotalCost);

        _mockBidCalculationService.Verify(s => s.CalculateTotalPrice(request), Times.Once);
    }

    [Fact]
    public void CalculateBid_WithLuxuryVehicle_ShouldReturnOkResult()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 2000m,
            VehicleType = VehicleTypeEnum.Luxury
        };

        var expectedResponse = new BidCalculationResponseDto
        {
            VehiclePrice = 2000m,
            VehicleType = VehicleTypeEnum.Luxury,
            BasicBuyerFee = 200m,
            SellerSpecialFee = 80m,
            AssociationFee = 15m,
            StorageFee = 100m,
            TotalCost = 2395m
        };

        _mockBidCalculationService
            .Setup(s => s.CalculateTotalPrice(request))
            .Returns(expectedResponse);

        // Act
        var result = _controller.CalculateBid(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualResponse = Assert.IsType<BidCalculationResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse, actualResponse);
        _mockBidCalculationService.Verify(s => s.CalculateTotalPrice(request), Times.Once);
    }

    [Fact]
    public void GetVehicleTypes_ShouldReturnOkResultWithAllVehicleTypes()
    {
        // Act
        var result = _controller.GetVehicleTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var vehicleTypes = Assert.IsType<VehicleTypeDto[]>(okResult.Value);

        Assert.Equal(2, vehicleTypes.Length);

        // Verify Common vehicle type
        var commonType = vehicleTypes.FirstOrDefault(vt => vt.Value == "Common");
        Assert.NotNull(commonType);
        Assert.Equal("Common", commonType.Value);
        Assert.Equal("Common", commonType.Label);

        // Verify Luxury vehicle type
        var luxuryType = vehicleTypes.FirstOrDefault(vt => vt.Value == "Luxury");
        Assert.NotNull(luxuryType);
        Assert.Equal("Luxury", luxuryType.Value);
        Assert.Equal("Luxury", luxuryType.Label);
    }

    [Fact]
    public void GetVehicleTypes_ShouldNotCallBidCalculationService()
    {
        // Act
        var result = _controller.GetVehicleTypes();

        // Assert
        Assert.IsType<OkObjectResult>(result);

        // Verify that no service method was called since GetVehicleTypes doesn't use the service
        _mockBidCalculationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void CalculateBid_WhenServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = -100m,
            VehicleType = VehicleTypeEnum.Common
        };

        _mockBidCalculationService
            .Setup(s => s.CalculateTotalPrice(request))
            .Throws(new ArgumentException("Vehicle price cannot be negative."));

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _controller.CalculateBid(request));
        Assert.Equal("Vehicle price cannot be negative.", exception.Message);

        _mockBidCalculationService.Verify(s => s.CalculateTotalPrice(request), Times.Once);
    }
}
