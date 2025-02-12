using System;
using System.Collections.Generic;

namespace CompanyAPI.Models;

public partial class Cargo
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? CompanyId { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? PaidAt { get; set; }
}
