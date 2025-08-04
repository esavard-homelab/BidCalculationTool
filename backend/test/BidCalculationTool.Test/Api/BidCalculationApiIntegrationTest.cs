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

        // Verify that the feeBreakdown structure is present
        Assert.True(responseObject.TryGetProperty("feeBreakdown", out var feeBreakdown));
        Assert.True(feeBreakdown.GetArrayLength() > 0);

        // Verify feeBreakdown contains expected fee items
        var feeBreakdownArray = feeBreakdown.EnumerateArray().ToArray();
        Assert.Contains(feeBreakdownArray, fee => fee.GetProperty("name").GetString() == "BasicBuyerFee");
        Assert.Contains(feeBreakdownArray, fee => fee.GetProperty("name").GetString() == "SpecialFee");
        Assert.Contains(feeBreakdownArray, fee => fee.GetProperty("name").GetString() == "AssociationFee");
        Assert.Contains(feeBreakdownArray, fee => fee.GetProperty("name").GetString() == "StorageFee");

        // Verify each fee item has required properties
        foreach (var fee in feeBreakdownArray)
        {
            Assert.True(fee.TryGetProperty("name", out _));
            Assert.True(fee.TryGetProperty("displayName", out _));
            Assert.True(fee.TryGetProperty("amount", out _));
            Assert.True(fee.TryGetProperty("description", out _));
        }
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
            "vehiclePrice", "vehicleType", "totalCost", "feeBreakdown"
        };

        foreach (var propertyName in expectedProperties)
        {
            Assert.True(responseObject.TryGetProperty(propertyName, out var property),
                $"Missing expected property: {propertyName}");

            // All numeric properties should be numbers (not strings)
            if (propertyName != "vehicleType" && propertyName != "feeBreakdown")
            {
                Assert.True(property.ValueKind == JsonValueKind.Number,
                    $"Property {propertyName} should be numeric");
            }
        }

        // Verify feeBreakdown structure specifically
        Assert.True(responseObject.TryGetProperty("feeBreakdown", out var feeBreakdown));
        Assert.True(feeBreakdown.ValueKind == JsonValueKind.Array,
            "feeBreakdown should be an array");

        var feeBreakdownArray = feeBreakdown.EnumerateArray().ToArray();
        Assert.True(feeBreakdownArray.Length >= 4,
            "feeBreakdown should contain at least 4 fee items");

        // Verify each fee item in breakdown has the correct structure
        foreach (var fee in feeBreakdownArray)
        {
            Assert.True(fee.TryGetProperty("name", out var name) && name.ValueKind == JsonValueKind.String,
                "Each fee should have a string 'name' property");
            Assert.True(fee.TryGetProperty("displayName", out var displayName) && displayName.ValueKind == JsonValueKind.String,
                "Each fee should have a string 'displayName' property");
            Assert.True(fee.TryGetProperty("amount", out var amount) && amount.ValueKind == JsonValueKind.Number,
                "Each fee should have a numeric 'amount' property");
            Assert.True(fee.TryGetProperty("description", out var description),
                "Each fee should have a 'description' property");
        }
    }
}
