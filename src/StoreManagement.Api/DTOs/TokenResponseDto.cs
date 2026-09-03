namespace StoreManagement.Api.DTOs;

public sealed record TokenResponseDto
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
}