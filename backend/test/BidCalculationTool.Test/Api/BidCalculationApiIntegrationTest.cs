using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BidCalculationTool.Test.Api;

public class BidCalculationApiIntegrationTest(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CalculateBid_WithValidRequest_ShouldReturnSuccessAndValidResponse()
    {
        // Arrange - Test the actual contract between frontend and backend
        var request = new
        {
            vehiclePrice = 1000.00m,
            vehicleType = "COMMON"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/BidCalculation", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

        // Verify the structure matches what frontend expects
        Assert.True(responseObject.TryGetProperty("vehiclePrice", out var vehiclePrice));
        Assert.Equal(1000.00m, vehiclePrice.GetDecimal());

        Assert.True(responseObject.TryGetProperty("totalCost", out var totalCost));
        Assert.True(totalCost.GetDecimal() > 1000.00m); // Should be greater than base price

        // Verify all fee properties are present (frontend needs them)
        Assert.True(responseObject.TryGetProperty("basicBuyerFee", out _));
        Assert.True(responseObject.TryGetProperty("sellerSpecialFee", out _));
        Assert.True(responseObject.TryGetProperty("associationFee", out _));
        Assert.True(responseObject.TryGetProperty("storageFee", out _));
    }

    [Theory]
    [InlineData("COMMON")]
    [InlineData("LUXURY")]
    [InlineData("common")]
    [InlineData("luxury")]
    [InlineData("Common")]
    [InlineData("Luxury")]
    public async Task CalculateBid_WithDifferentVehicleTypeFormats_ShouldAcceptAll(string vehicleType)
    {
        // Arrange - Test that backend accepts all format variations from frontend
        var request = new
        {
            vehiclePrice = 2000.00m,
            vehicleType
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/BidCalculation", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

        Assert.True(responseObject.TryGetProperty("vehiclePrice", out var vehiclePrice));
        Assert.Equal(2000.00m, vehiclePrice.GetDecimal());
    }

    [Fact]
    public async Task CalculateBid_WithInvalidVehicleType_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            vehiclePrice = 1000.00m,
            vehicleType = "INVALID_TYPE"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/BidCalculation", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetVehicleTypes_ShouldReturnExpectedFormat()
    {
        // Act
        var response = await _client.GetAsync("/api/BidCalculation/vehicle-types");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var vehicleTypes = JsonSerializer.Deserialize<JsonElement[]>(responseContent);

        Assert.Equal(2, vehicleTypes!.Length);

        // Verify the format matches what frontend expects
        foreach (var vehicleType in vehicleTypes)
        {
            Assert.True(vehicleType.TryGetProperty("value", out _));
            Assert.True(vehicleType.TryGetProperty("label", out _));
        }

        // Verify specific values
        var commonType = vehicleTypes.FirstOrDefault(vt =>
            vt.GetProperty("value").GetString() == "Common");
        Assert.True(commonType.ValueKind != JsonValueKind.Undefined, "Common vehicle type should be present");

        var luxuryType = vehicleTypes.FirstOrDefault(vt =>
            vt.GetProperty("value").GetString() == "Luxury");
        Assert.True(luxuryType.ValueKind != JsonValueKind.Undefined, "Luxury vehicle type should be present");
    }

    [Fact]
    public async Task ApiContract_ShouldMatchExpectedShape()
    {
        // Arrange - This test verifies the complete contract shape
        var request = new
        {
            vehiclePrice = 1500.00m,
            vehicleType = "LUXURY"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/BidCalculation", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

        // Verify ALL properties that frontend expects are present with correct types
        var expectedProperties = new[]
        {
            "vehiclePrice", "vehicleType", "basicBuyerFee",
            "sellerSpecialFee", "associationFee", "storageFee", "totalCost"
        };

        foreach (var propertyName in expectedProperties)
        {
            Assert.True(responseObject.TryGetProperty(propertyName, out var property),
                $"Missing expected property: {propertyName}");

            // All numeric properties should be numbers (not strings)
            if (propertyName != "vehicleType")
            {
                Assert.True(property.ValueKind == JsonValueKind.Number,
                    $"Property {propertyName} should be numeric");
            }
        }
    }
}
