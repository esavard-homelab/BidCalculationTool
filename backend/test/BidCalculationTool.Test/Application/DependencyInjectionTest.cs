using BidCalculationTool.Application;
using BidCalculationTool.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BidCalculationTool.Test.Application;

public class DependencyInjectionTest
{
    [Fact]
    public void AddApplication_ShouldRegisterBidCalculationService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplication();

        // Assert
        Assert.Same(services, result); // Should return the same service collection for fluent interface

        // Build service provider to verify registrations
        var serviceProvider = services.BuildServiceProvider();

        // Verify that IBidCalculationService is registered and can be resolved
        var bidCalculationService = serviceProvider.GetService<IBidCalculationService>();
        Assert.NotNull(bidCalculationService);
        Assert.IsType<BidCalculationService>(bidCalculationService);
    }

    [Fact]
    public void AddApplication_ShouldRegisterServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Create two scopes and verify that each scope gets the same instance
        // but different scopes get different instances
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var service1a = scope1.ServiceProvider.GetRequiredService<IBidCalculationService>();
        var service1b = scope1.ServiceProvider.GetRequiredService<IBidCalculationService>();
        var service2 = scope2.ServiceProvider.GetRequiredService<IBidCalculationService>();

        // Same scope should return same instance (scoped lifetime)
        Assert.Same(service1a, service1b);

        // Different scopes should return different instances
        Assert.NotSame(service1a, service2);
    }

    [Fact]
    public void AddApplication_ShouldRegisterCorrectServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBidCalculationService));

        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(IBidCalculationService), serviceDescriptor.ServiceType);
        Assert.Equal(typeof(BidCalculationService), serviceDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplication_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services!.AddApplication());
    }

    [Fact]
    public void AddApplication_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Call AddApplication multiple times
        services.AddApplication();
        services.AddApplication();

        // Assert - Should have registered the service twice (last registration wins in DI)
        var serviceProvider = services.BuildServiceProvider();
        var bidCalculationService = serviceProvider.GetService<IBidCalculationService>();

        Assert.NotNull(bidCalculationService);
        Assert.IsType<BidCalculationService>(bidCalculationService);

        // Verify we have multiple registrations
        var serviceDescriptors = services.Where(s => s.ServiceType == typeof(IBidCalculationService)).ToList();
        Assert.Equal(2, serviceDescriptors.Count);
    }
}
