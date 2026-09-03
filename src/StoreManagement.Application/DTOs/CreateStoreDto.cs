using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManagement.Application.DTOs
{
    public sealed record CreateStoreDto
    {
        public required string Name { get; init; }
        public string? Address { get; init; }
        public string? Country { get; init; }
        public string? Timezone { get; init; }
    }
}
