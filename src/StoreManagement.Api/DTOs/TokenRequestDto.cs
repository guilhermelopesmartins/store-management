namespace StoreManagement.Api.DTOs;

public sealed record TokenRequestDto
{
    public required Guid CompanyId { get; init; }
}