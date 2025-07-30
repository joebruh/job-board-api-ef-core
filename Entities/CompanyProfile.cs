using System;

namespace EntityApi.Entities;

public class CompanyProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;

    // public ICollection<Job> Jobs { get; set; } = new List<Job>();


}
