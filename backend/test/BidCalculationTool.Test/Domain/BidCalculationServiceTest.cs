using BidCalculationTool.Domain.Services;
using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Enums;
using BidCalculationTool.Domain.FeeStrategies;
using Moq;

namespace BidCalculationTool.Test.Domain;

public class BidCalculationServiceTest
{
    private readonly Mock<IFeeStrategy> _mockBasicFeeStrategy;
    private readonly Mock<IFeeStrategy> _mockSpecialFeeStrategy;
    private readonly Mock<IFeeStrategy> _mockAssociationFeeStrategy;
    private readonly Mock<IFeeStrategy> _mockStorageFeeStrategy;
    private readonly BidCalculationService _service;

    public BidCalculationServiceTest()
    {
        // Create mocked fee strategies
        _mockBasicFeeStrategy = new Mock<IFeeStrategy>();
        _mockSpecialFeeStrategy = new Mock<IFeeStrategy>();
        _mockAssociationFeeStrategy = new Mock<IFeeStrategy>();
        _mockStorageFeeStrategy = new Mock<IFeeStrategy>();

        // Setup BasicBuyerFee strategy
        _mockBasicFeeStrategy.Setup(x => x.FeeName).Returns("BasicBuyerFee");
        _mockBasicFeeStrategy.Setup(x => x.DisplayName).Returns("Basic Buyer Fee");
        _mockBasicFeeStrategy.Setup(x => x.Description).Returns("10% of vehicle price with minimum and maximum limits based on vehicle type");

        // Setup SpecialFee strategy
        _mockSpecialFeeStrategy.Setup(x => x.FeeName).Returns("SpecialFee");
        _mockSpecialFeeStrategy.Setup(x => x.DisplayName).Returns("Seller's Special Fee");
        _mockSpecialFeeStrategy.Setup(x => x.Description).Returns("2% for Common vehicles, 4% for Luxury vehicles");

        // Setup AssociationFee strategy
        _mockAssociationFeeStrategy.Setup(x => x.FeeName).Returns("AssociationFee");
        _mockAssociationFeeStrategy.Setup(x => x.DisplayName).Returns("Association Fee");
        _mockAssociationFeeStrategy.Setup(x => x.Description).Returns("Tiered fee based on vehicle price ranges");

        // Setup StorageFee strategy
        _mockStorageFeeStrategy.Setup(x => x.FeeName).Returns("StorageFee");
        _mockStorageFeeStrategy.Setup(x => x.DisplayName).Returns("Storage Fee");
        _mockStorageFeeStrategy.Setup(x => x.Description).Returns("Fixed storage fee for all vehicles");

        var feeStrategies = new List<IFeeStrategy>
        {
            _mockBasicFeeStrategy.Object,
            _mockSpecialFeeStrategy.Object,
            _mockAssociationFeeStrategy.Object,
            _mockStorageFeeStrategy.Object
        };

        _service = new BidCalculationService(feeStrategies);
    }

    [Fact]
    public void CalculateTotalPrice_ShouldOrchestrateFeeStrategiesCorrectly()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 1000.00m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Setup mock returns
        _mockBasicFeeStrategy.Setup(x => x.Calculate(1000.00m, VehicleTypeEnum.Common)).Returns(50.00m);
        _mockSpecialFeeStrategy.Setup(x => x.Calculate(1000.00m, VehicleTypeEnum.Common)).Returns(20.00m);
        _mockAssociationFeeStrategy.Setup(x => x.Calculate(1000.00m, VehicleTypeEnum.Common)).Returns(10.00m);
        _mockStorageFeeStrategy.Setup(x => x.Calculate(1000.00m, VehicleTypeEnum.Common)).Returns(100.00m);

        // Act
        var result = _service.CalculateTotalPrice(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000.00m, result.VehiclePrice);
        Assert.Equal(VehicleTypeEnum.Common, result.VehicleType);
        Assert.Equal(1180.00m, result.TotalCost); // 1000 + 50 + 20 + 10 + 100

        // Verify FeeBreakdown is populated correctly
        Assert.NotNull(result.FeeBreakdown);
        Assert.Equal(4, result.FeeBreakdown.Count);

        var basicFeeItem = result.FeeBreakdown.First(f => f.Name == "BasicBuyerFee");
        Assert.Equal("Basic Buyer Fee", basicFeeItem.DisplayName);
        Assert.Equal(50.00m, basicFeeItem.Amount);
        Assert.NotNull(basicFeeItem.Description);

        // Verify all strategies were called
        _mockBasicFeeStrategy.Verify(x => x.Calculate(1000.00m, VehicleTypeEnum.Common), Times.Once);
        _mockSpecialFeeStrategy.Verify(x => x.Calculate(1000.00m, VehicleTypeEnum.Common), Times.Once);
        _mockAssociationFeeStrategy.Verify(x => x.Calculate(1000.00m, VehicleTypeEnum.Common), Times.Once);
        _mockStorageFeeStrategy.Verify(x => x.Calculate(1000.00m, VehicleTypeEnum.Common), Times.Once);
    }

    [Fact]
    public void CalculateTotalPrice_ShouldCallAllStrategiesWithSameParameters()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 2500.00m,
            VehicleType = VehicleTypeEnum.Luxury
        };

        // Setup mock returns (values don't matter for this test)
        _mockBasicFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(100.00m);
        _mockSpecialFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(50.00m);
        _mockAssociationFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(15.00m);
        _mockStorageFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(100.00m);

        // Act
        _service.CalculateTotalPrice(request);

        // Assert - Verify all strategies received the same parameters
        _mockBasicFeeStrategy.Verify(x => x.Calculate(2500.00m, VehicleTypeEnum.Luxury), Times.Once);
        _mockSpecialFeeStrategy.Verify(x => x.Calculate(2500.00m, VehicleTypeEnum.Luxury), Times.Once);
        _mockAssociationFeeStrategy.Verify(x => x.Calculate(2500.00m, VehicleTypeEnum.Luxury), Times.Once);
        _mockStorageFeeStrategy.Verify(x => x.Calculate(2500.00m, VehicleTypeEnum.Luxury), Times.Once);
    }

    [Fact]
    public void CalculateTotalPrice_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 500.00m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Setup different fee amounts to test total calculation
        _mockBasicFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(25.00m);
        _mockSpecialFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(15.00m);
        _mockAssociationFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(8.00m);
        _mockStorageFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>())).Returns(100.00m);

        // Act
        var result = _service.CalculateTotalPrice(request);

        // Assert
        var expectedTotal = 500.00m + 25.00m + 15.00m + 8.00m + 100.00m; // 648.00
        Assert.Equal(expectedTotal, result.TotalCost);
    }

    [Fact]
    public void Constructor_WithNoStrategies_ShouldThrowArgumentException()
    {
        // Arrange
        // ReSharper disable once CollectionNeverUpdated.Local
        var emptyStrategies = new List<IFeeStrategy>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BidCalculationService(emptyStrategies));
        Assert.Equal("No fee strategies registered. Please register at least one fee strategy.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullStrategies_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BidCalculationService(null!));
    }

    [Fact]
    public void CalculateTotalPrice_WhenStrategyThrowsException_ShouldPropagateException()
    {
        // Arrange
        var request = new BidCalculationRequestDto
        {
            VehiclePrice = 1000.00m,
            VehicleType = VehicleTypeEnum.Common
        };

        // Setup one strategy to throw an exception
        _mockBasicFeeStrategy.Setup(x => x.Calculate(It.IsAny<decimal>(), It.IsAny<VehicleTypeEnum>()))
                            .Throws(new ArgumentException("Invalid vehicle type"));

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CalculateTotalPrice(request));
        Assert.Equal("Invalid vehicle type", exception.Message);
    }
}
