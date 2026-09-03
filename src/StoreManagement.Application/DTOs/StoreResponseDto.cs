using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManagement.Application.DTOs
{
    public sealed record StoreResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid CompanyId { get; init; }
        public required string Name { get; init; }
        public string? Address { get; init; }
        public string? Country { get; init; }
        public string? Timezone { get; init; }
        public required bool IsActive { get; init; }
    }
}
