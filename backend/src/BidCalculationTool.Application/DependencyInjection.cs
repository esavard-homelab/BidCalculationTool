using BidCalculationTool.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BidCalculationTool.Application;

/// <summary>
/// Dependency injection configuration for the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBidCalculationService, BidCalculationService>();
        return services;
    }
}
