using System;

namespace EntityApi.Core.Entities;

public class JobPost
{
    public int Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }


    public CompanyProfile CompanyProfile { get; set; } = default!;
}
