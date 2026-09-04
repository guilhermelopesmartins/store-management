using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreManagement.Api.Controllers;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;
using StoreManagement.Domain.Exceptions;
using Xunit;

namespace StoreManagement.UnitTests.Controllers;

public class StoresControllerTests
{
    private readonly Mock<IStoreService> _storeServiceMock;
    private readonly StoresController _sut;

    public StoresControllerTests()
    {
        _storeServiceMock = new Mock<IStoreService>();
        _sut = new StoresController(_storeServiceMock.Object);
    }

    [Fact]
    public async Task CreateStore_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var dto = new CreateStoreDto { Name = "Loja Centro" };

        var expectedResponse = new StoreResponseDto
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = dto.Name,
            IsActive = true
        };

        _storeServiceMock
            .Setup(s => s.CreateStoreAsync(companyId, dto))
            .ReturnsAsync(expectedResponse);

        // simula a claim companyId no HttpContext do controller
        _sut.SetCompanyIdClaim(companyId);

        // Act
        var result = await _sut.CreateStore(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().Be(expectedResponse);
        createdResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenStoreExists()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var expectedResponse = new StoreResponseDto
        {
            Id = storeId,
            CompanyId = companyId,
            Name = "Loja Centro",
            IsActive = true
        };

        _storeServiceMock
            .Setup(s => s.GetByIdAsync(storeId, companyId))
            .ReturnsAsync(expectedResponse);

        _sut.SetCompanyIdClaim(companyId);

        var result = await _sut.GetById(storeId);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetById_ShouldPropagateStoreNotFoundException_WhenStoreDoesNotExist()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeServiceMock
            .Setup(s => s.GetByIdAsync(storeId, companyId))
            .ThrowsAsync(new StoreNotFoundException(storeId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.GetById(storeId);

        await act.Should().ThrowAsync<StoreNotFoundException>();
    }

    [Fact]
    public async Task GetById_ShouldPropagateStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeServiceMock
            .Setup(s => s.GetByIdAsync(storeId, companyId))
            .ThrowsAsync(new StoreAccessDeniedException(storeId, companyId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.GetById(storeId);

        await act.Should().ThrowAsync<StoreAccessDeniedException>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithStores()
    {
        var companyId = Guid.NewGuid();

        var stores = new List<StoreResponseDto>
    {
        new() { Id = Guid.NewGuid(), CompanyId = companyId, Name = "Loja A", IsActive = true }
    };

        _storeServiceMock
            .Setup(s => s.GetAllAsync(companyId))
            .ReturnsAsync(stores);

        _sut.SetCompanyIdClaim(companyId);

        var result = await _sut.GetAll();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(stores);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenStoreExists()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var dto = new UpdateStoreDto { Name = "Loja Nova", IsActive = true };

        var expectedResponse = new StoreResponseDto
        {
            Id = storeId,
            CompanyId = companyId,
            Name = dto.Name,
            IsActive = true
        };

        _storeServiceMock
            .Setup(s => s.UpdateAsync(storeId, companyId, dto))
            .ReturnsAsync(expectedResponse);

        _sut.SetCompanyIdClaim(companyId);

        var result = await _sut.Update(storeId, dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldPropagateStoreNotFoundException_WhenStoreDoesNotExist()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var dto = new UpdateStoreDto { Name = "Loja Nova", IsActive = true };

        _storeServiceMock
            .Setup(s => s.UpdateAsync(storeId, companyId, dto))
            .ThrowsAsync(new StoreNotFoundException(storeId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.Update(storeId, dto);

        await act.Should().ThrowAsync<StoreNotFoundException>();
    }

    [Fact]
    public async Task Update_ShouldPropagateStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        var dto = new UpdateStoreDto { Name = "Loja Nova", IsActive = true };

        _storeServiceMock
            .Setup(s => s.UpdateAsync(storeId, companyId, dto))
            .ThrowsAsync(new StoreAccessDeniedException(storeId, companyId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.Update(storeId, dto);

        await act.Should().ThrowAsync<StoreAccessDeniedException>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenStoreExists()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeServiceMock
            .Setup(s => s.DeleteAsync(storeId, companyId))
            .Returns(Task.CompletedTask);

        _sut.SetCompanyIdClaim(companyId);

        var result = await _sut.Delete(storeId);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldPropagateStoreNotFoundException_WhenStoreDoesNotExist()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeServiceMock
            .Setup(s => s.DeleteAsync(storeId, companyId))
            .ThrowsAsync(new StoreNotFoundException(storeId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.Delete(storeId);

        await act.Should().ThrowAsync<StoreNotFoundException>();
    }

    [Fact]
    public async Task Delete_ShouldPropagateStoreAccessDeniedException_WhenStoreBelongsToAnotherCompany()
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        _storeServiceMock
            .Setup(s => s.DeleteAsync(storeId, companyId))
            .ThrowsAsync(new StoreAccessDeniedException(storeId, companyId));

        _sut.SetCompanyIdClaim(companyId);

        var act = async () => await _sut.Delete(storeId);

        await act.Should().ThrowAsync<StoreAccessDeniedException>();
    }
}
