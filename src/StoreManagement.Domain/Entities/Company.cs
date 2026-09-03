using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManagement.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }

    public ICollection<Store> Stores { get; set; } = new List<Store>();
}
