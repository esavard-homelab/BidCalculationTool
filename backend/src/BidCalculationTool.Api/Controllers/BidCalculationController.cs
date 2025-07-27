using BidCalculationTool.Domain.Dto;
using BidCalculationTool.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using BidCalculationTool.Domain.Enums;

namespace BidCalculationTool.Api.Controllers;

/// <summary>
/// Controller for handling bid calculations.
/// This controller provides endpoints to calculate total price based on bid requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BidCalculationController : ControllerBase
{
    private readonly IBidCalculationService _bidCalculationService;

    /// <summary>
    /// Constructor for BidCalculationController
    /// Initializes a new instance of the <see cref="BidCalculationController"/> class.
    /// </summary>
    /// <param name="bidCalculationService">Injected service that calculate fees and total price.</param>
    public BidCalculationController(IBidCalculationService bidCalculationService)
    {
        _bidCalculationService = bidCalculationService;
    }

    /// <summary>
    /// Calculates the total price based on the provided bid request.
    /// </summary>
    /// <param name="request">The <see cref="BidCalculationRequestDto"/> required for the calculation.</param>
    /// <returns><see cref="BidCalculationResponseDto"/></returns>
    [HttpPost]
    public IActionResult CalculateBid([FromBody] BidCalculationRequestDto request)
    {
        var response = _bidCalculationService.CalculateTotalPrice(request);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the list of available vehicle types.
    /// </summary>
    /// <returns><see cref="IActionResult"/> that contains all vehicle types and descriptions</returns>
    [HttpGet("vehicle-types")]
    public IActionResult GetVehicleTypes()
    {
        var vehicleTypes = Enum.GetValues<VehicleTypeEnum>()
            .Select(vt => new VehicleTypeDto
            {
                Value = vt.ToDisplayString(),
                Label = vt.ToDisplayString()
            })
            .ToArray();

        return Ok(vehicleTypes);
    }
}
