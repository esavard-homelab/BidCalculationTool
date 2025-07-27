using BidCalculationTool.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Add custom JSON converter for VehicleTypeEnum
        options.JsonSerializerOptions.Converters.Add(new BidCalculationTool.Api.Converters.VehicleTypeEnumJsonConverter());
        // Configure to use camelCase for property names
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

// Inject the domain services
builder.Services.AddApplication();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bid Calculation API", Version = "v1" });
});

// Add CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

// Map controllers instead of individual endpoints
app.MapControllers();

app.Run();

/// <summary>
/// Program class for ASP.NET Core application entry point.
/// This partial class is required for integration tests with WebApplicationFactory.
/// </summary>
public partial class Program { }
