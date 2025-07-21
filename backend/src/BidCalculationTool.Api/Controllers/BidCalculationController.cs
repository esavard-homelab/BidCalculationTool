using BidCalculationTool.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidCalculationController : ControllerBase
{
    [HttpPost]
    public IActionResult CalculateBid([FromBody] BidCalculationRequestDto request)
    {
        // TODO: Implémenter la logique de calcul
        // Vous pourrez injecter vos services Application ici

        return Ok(new BidCalculationResponseDto
        {
            VehiclePrice = request.VehiclePrice,
            VehicleType = request.VehicleType,
            BasicBuyerFee = 0, // TODO: Calculer
            SellerSpecialFee = 0, // TODO: Calculer
            AssociationFee = 0, // TODO: Calculer
            StorageFee = 100, // Fixe
            TotalCost = 0 // TODO: Calculer
        });
    }

    [HttpGet("vehicle-types")]
    public IActionResult GetVehicleTypes()
    {
        var vehicleTypes = Enum.GetValues<VehicleType>()
            .Select(vt => new VehicleTypeDto
            {
                Value = vt.ToDisplayString(),
                Label = vt.ToDisplayString()
            })
            .ToArray();

        return Ok(vehicleTypes);
    }
}
