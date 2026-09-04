using FluentAssertions;
using Moq;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Exceptions;
using StoreManagement.Domain.Repositories;
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
            .Setup(r => r.GetByIdReadOnlyAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _sut.GetByIdAsync(storeId, companyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(storeId);
        result.Name.Should().Be(existingStore.Name);
    }

    [Fact]
    public async Task GetById_ShouldThrowStoreNotFoundException_WhenStoreDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeRepositoryMock
            .Setup(r => r.GetByIdReadOnlyAsync(storeId))
            .ReturnsAsync((Store?)null);

        // Act
        var act = async () => await _sut.GetByIdAsync(storeId, companyId);

        // Assert
        await act.Should().ThrowAsync<StoreNotFoundException>();
    }

    [Fact]
    public async Task GetById_ShouldThrowStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var existingStore = new Store
        {
            Id = storeId,
            CompanyId = Guid.NewGuid(),
            Name = "Loja Centro",
            IsActive = true
        };

        _storeRepositoryMock
            .Setup(r => r.GetByIdReadOnlyAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var act = async () => await _sut.GetByIdAsync(storeId, companyId);

        // Assert
        await act.Should().ThrowAsync<StoreAccessDeniedException>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOnlyStoresBelongingToCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        var stores = new List<Store>
    {
        new() { Id = Guid.NewGuid(), CompanyId = companyId, Name = "Loja A", IsActive = true },
        new() { Id = Guid.NewGuid(), CompanyId = companyId, Name = "Loja B", IsActive = true }
    };

        _storeRepositoryMock
            .Setup(r => r.GetAllAsync(companyId))
            .ReturnsAsync(stores);

        // Act
        var result = await _sut.GetAllAsync(companyId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(s => s.CompanyId == companyId);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmpty_WhenCompanyHasNoStores()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _storeRepositoryMock
            .Setup(r => r.GetAllAsync(companyId))
            .ReturnsAsync(new List<Store>());

        // Act
        var result = await _sut.GetAllAsync(companyId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedStore_WhenStoreExistsAndBelongsToCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var existingStore = new Store
        {
            Id = storeId,
            CompanyId = companyId,
            Name = "Loja Antiga",
            IsActive = true
        };

        var dto = new UpdateStoreDto
        {
            Name = "Loja Nova",
            Country = "BR",
            IsActive = true
        };

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        _storeRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Store>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAsync(storeId, companyId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Country.Should().Be(dto.Country);

        _storeRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Store>(s => s.Name == dto.Name)), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldThrowStoreNotFoundException_WhenStoreDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var dto = new UpdateStoreDto { Name = "Loja Nova", IsActive = true };

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((Store?)null);

        // Act
        var act = async () => await _sut.UpdateAsync(storeId, companyId, dto);

        // Assert
        await act.Should().ThrowAsync<StoreNotFoundException>();
        _storeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Store>()), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldThrowStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var existingStore = new Store
        {
            Id = storeId,
            CompanyId = Guid.NewGuid(),
            Name = "Loja Antiga",
            IsActive = true
        };

        var dto = new UpdateStoreDto { Name = "Loja Nova", IsActive = true };

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var act = async () => await _sut.UpdateAsync(storeId, companyId, dto);

        // Assert
        await act.Should().ThrowAsync<StoreAccessDeniedException>();
        _storeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Store>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldCallRepositoryDelete_WhenStoreExistsAndBelongsToCompany()
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
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        _storeRepositoryMock
            .Setup(r => r.DeleteAsync(existingStore))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteAsync(storeId, companyId);

        // Assert
        _storeRepositoryMock.Verify(r => r.DeleteAsync(existingStore), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldThrowStoreNotFoundException_WhenStoreDoesNotExist()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((Store?)null);

        // Act
        var act = async () => await _sut.DeleteAsync(storeId, companyId);

        // Assert
        await act.Should().ThrowAsync<StoreNotFoundException>();
        _storeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Store>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldThrowStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var existingStore = new Store
        {
            Id = storeId,
            CompanyId = Guid.NewGuid(),
            Name = "Loja Centro",
            IsActive = true
        };

        _storeRepositoryMock
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var act = async () => await _sut.DeleteAsync(storeId, companyId);

        // Assert
        await act.Should().ThrowAsync<StoreAccessDeniedException>();
        _storeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Store>()), Times.Never);
    }
}
