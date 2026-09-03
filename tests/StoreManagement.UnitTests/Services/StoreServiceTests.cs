using FluentAssertions;
using Moq;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Repositories;
using System.Timers;
using Xunit;

namespace StoreManagement.UnitTests.Services;

public class StoreServiceTests
{
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly IStoreService _sut; // system under test

    public StoreServiceTests()
    {
        _storeRepositoryMock = new Mock<IStoreRepository>();
        _sut = new StoreService(_storeRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateStore_ShouldReturnCreatedStore_WhenDataIsValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var dto = new CreateStoreDto
        {
            Name = "Loja Centro",
            Country = "BR",
            Timezone = "America/Sao_Paulo"
        };

        _storeRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Store>()))
            .ReturnsAsync((Store s) => s);

        // Act
        var result = await _sut.CreateStoreAsync(companyId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.CompanyId.Should().Be(companyId);
        result.IsActive.Should().BeTrue();

        _storeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Store>()), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnStore_WhenStoreExistsAndBelongsToCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var existingStore = new Store
        {
            Id = storeId,
            CompanyId = companyId,
            Name = "Loja Centro",
            IsActive = true
        };

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId, companyId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _sut.GetByIdAsync(storeId, companyId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(storeId);
        result.Name.Should().Be(existingStore.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenStoreDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId, companyId))
            .ReturnsAsync((Store?)null);

        // Act
        var result = await _sut.GetByIdAsync(storeId, companyId);

        // Assert
        result.Should().BeNull();
    }
}