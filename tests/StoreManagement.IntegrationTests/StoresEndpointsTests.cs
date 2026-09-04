using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Api.DTOs;
using StoreManagement.Application.DTOs;
using StoreManagement.IntegrationTests.Infrastructure;
using Xunit;

namespace StoreManagement.IntegrationTests;

public class StoresEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web); // PropertyNameCaseInsensitive = true, camelCase-aware

    public StoresEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Guid> AuthenticateAsync(Guid companyId)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token", new TokenRequestDto
        {
            CompanyId = companyId
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>(JsonOptions);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body!.Token);

        return companyId;
    }

    [Fact]
    public async Task CreateStore_ThenGetById_ShouldReturnTheCreatedStore()
    {
        var companyId = Guid.NewGuid();
        await AuthenticateAsync(companyId);

        var createDto = new CreateStoreDto
        {
            Name = "Loja Integração",
            Country = "BR",
            Timezone = "America/Sao_Paulo"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/stores", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);
        created.Should().NotBeNull();
        created!.Name.Should().Be(createDto.Name);
        created.CompanyId.Should().Be(companyId);

        var getResponse = await _client.GetAsync($"/api/stores/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenStoreDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync(Guid.NewGuid());

        // Act
        var response = await _client.GetAsync($"/api/stores/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("Store not found");
    }

    [Fact]
    public async Task CreateStore_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/stores", new CreateStoreDto
        {
            Name = "Loja Sem Token"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_ShouldReturnForbidden_WhenStoreBelongsToAnotherCompany()
    {
        // Arrange - cria uma store pra company A
        var companyA = Guid.NewGuid();
        await AuthenticateAsync(companyA);

        var createResponse = await _client.PostAsJsonAsync("/api/stores", new CreateStoreDto
        {
            Name = "Loja da Empresa A"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);

        // Act - autentica como company B e tenta acessar a store da A
        await AuthenticateAsync(Guid.NewGuid());
        var getResponse = await _client.GetAsync($"/api/stores/{created!.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        getResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await getResponse.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be((int)HttpStatusCode.Forbidden);
        problem.Title.Should().Be("Access denied");
    }

    [Fact]
    public async Task Update_ThenGetById_ShouldReturnTheUpdatedStore()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        await AuthenticateAsync(companyId);

        var createResponse = await _client.PostAsJsonAsync("/api/stores", new CreateStoreDto
        {
            Name = "Loja Antiga",
            Country = "BR"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);

        var updateDto = new UpdateStoreDto
        {
            Name = "Loja Nova",
            Country = "BR",
            IsActive = false
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"/api/stores/{created!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync($"/api/stores/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);

        // Assert
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be(updateDto.Name);
        fetched.IsActive.Should().BeFalse();
    }
}