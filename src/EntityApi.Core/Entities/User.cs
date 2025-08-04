using System;

namespace EntityApi.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // profiles
    public CompanyProfile? CompanyProfile { get; set; }
    public CandidateProfile? CandidateProfile { get; set; }
}
