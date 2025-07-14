var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// BidCalculation API endpoint for testing connection
app.MapGet("/BidCalculation/datetime", () => new
    {
        Message = "Connection successful to Bid Calculation Tool API",
        DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        ServerTimeZone = TimeZoneInfo.Local.DisplayName
    })
.WithName("GetBidCalculationDateTime")
.WithOpenApi()
.WithSummary("Test endpoint to verify API connectivity")
.WithDescription("Returns current server date/time for frontend connection testing");

app.Run();
