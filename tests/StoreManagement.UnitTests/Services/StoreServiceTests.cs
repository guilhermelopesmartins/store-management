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
}