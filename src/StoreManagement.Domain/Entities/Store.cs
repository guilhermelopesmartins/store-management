using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManagement.Domain.Entities;

public class Store
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? Timezone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
