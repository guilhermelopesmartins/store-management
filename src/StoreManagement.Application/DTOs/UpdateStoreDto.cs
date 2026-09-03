namespace StoreManagement.Application.DTOs;

public sealed record UpdateStoreDto
{
    public required string Name { get; init; }
    public string? Address { get; init; }
    public string? Country { get; init; }
    public string? Timezone { get; init; }
    public required bool IsActive { get; init; }
}