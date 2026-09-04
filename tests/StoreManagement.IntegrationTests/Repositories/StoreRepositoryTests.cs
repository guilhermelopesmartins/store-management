using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using StoreManagement.Infrastructure.Persistence;
using StoreManagement.Infrastructure.Repositories;
using Xunit;

namespace StoreManagement.IntegrationTests.Repositories;

public class StoreRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdReadOnlyAsync_ShouldNotTrackTheReturnedEntity()
    {
        // Arrange
        await using var context = CreateContext();
        var store = new Store { Id = Guid.NewGuid(), CompanyId = Guid.NewGuid(), Name = "Loja Centro" };
        context.Stores.Add(store);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var sut = new StoreRepository(context);

        // Act
        var result = await sut.GetByIdReadOnlyAsync(store.Id);

        // Assert
        result.Should().NotBeNull();
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldTrackTheReturnedEntity()
    {
        // Arrange
        await using var context = CreateContext();
        var store = new Store { Id = Guid.NewGuid(), CompanyId = Guid.NewGuid(), Name = "Loja Centro" };
        context.Stores.Add(store);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var sut = new StoreRepository(context);

        // Act
        var result = await sut.GetByIdAsync(store.Id);

        // Assert
        result.Should().NotBeNull();
        context.ChangeTracker.Entries().Should().ContainSingle(e => e.Entity == result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotTrackTheReturnedEntities()
    {
        // Arrange
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        context.Stores.AddRange(
            new Store { Id = Guid.NewGuid(), CompanyId = companyId, Name = "Loja A" },
            new Store { Id = Guid.NewGuid(), CompanyId = companyId, Name = "Loja B" });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var sut = new StoreRepository(context);

        // Act
        var result = await sut.GetAllAsync(companyId);

        // Assert
        result.Should().HaveCount(2);
        context.ChangeTracker.Entries().Should().BeEmpty();
    }
}
