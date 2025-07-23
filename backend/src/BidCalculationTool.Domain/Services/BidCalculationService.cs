using BidCalculationTool.Domain.Dto;

namespace BidCalculationTool.Domain.Services;

public class BidCalculationService : IBidCalculationService
{
    public BidCalculationResponseDto CalculateTotalPrice(BidCalculationRequestDto request)
    {
        var basicFee = CalculateBasicBuyerFee(request.VehiclePrice, request.VehicleType);
        var specialFee = CalculateSpecialFee(request.VehiclePrice, request.VehicleType);
        var associationFee = CalculateAssociationFee(request.VehiclePrice);
        const decimal storageFee = 100.00m; // TODO: Magic number, refactor
        var totalCost = request.VehiclePrice + basicFee + specialFee + associationFee + storageFee;

        return new BidCalculationResponseDto
        {
            VehiclePrice = request.VehiclePrice,
            VehicleType = request.VehicleType,
            BasicBuyerFee = basicFee,
            SellerSpecialFee = specialFee,
            AssociationFee = associationFee,
            StorageFee = storageFee,
            TotalCost = totalCost
        };
    }

    // TODO: Refactor the methods below to use a more elegant approach, e.g. using a dictionary or a configuration file,
    // and the strategy pattern.

    /// <summary>
    /// This method calculates the association fee based on the vehicle price.
    /// </summary>
    /// <param name="vehiclePrice"></param>
    /// <returns>The association fee</returns>
    /// <exception cref="ArgumentException"></exception>
    private static decimal CalculateAssociationFee(decimal vehiclePrice)
    {
        return vehiclePrice switch
        {
            > 0 and <= 500 => 5.00m,
            > 500 and <= 1000 => 10.00m,
            > 1000 and <= 3000 => 15.00m,
            > 3000 => 20.00m,
            _ => throw new ArgumentException("Vehicle price cannot be negative.")
        };
    }

    /// <summary>
    /// This method calculates the basic buyer fee based on the vehicle price and type.
    /// </summary>
    /// <param name="vehiclePrice"></param>
    /// <param name="vehicleType"></param>
    /// <returns>The basic buyer fee</returns>
    /// <exception cref="ArgumentException"></exception>
    private static decimal CalculateBasicBuyerFee(decimal vehiclePrice, string vehicleType)
    {
        return vehicleType switch
        {
            "Common" => Math.Max(10, Math.Min(50, vehiclePrice * 0.10m)),
            "Luxury" => Math.Max(25, Math.Min(200, vehiclePrice * 0.10m)),
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleType}")
        };
    }

    /// <summary>
    /// This method calculates the special fee based on the vehicle price and type.
    /// </summary>
    /// <param name="vehiclePrice"></param>
    /// <param name="vehicleType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static decimal CalculateSpecialFee(decimal vehiclePrice, string vehicleType)
    {
        return vehicleType switch
        {
            "Common" => vehiclePrice * 0.02m,
            "Luxury" => vehiclePrice * 0.04m,
            _ => throw new ArgumentException($"Invalid vehicle type: {vehicleType}")
        };
    }
}
