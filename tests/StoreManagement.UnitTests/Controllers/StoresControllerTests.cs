using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreManagement.Api.Controllers;
using StoreManagement.Application.DTOs;
using StoreManagement.Application.Services;
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
}