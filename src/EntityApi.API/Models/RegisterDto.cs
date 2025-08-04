using System;
using System.ComponentModel.DataAnnotations;

namespace EntityApi.API.Models;

public class RegisterDto : IValidatableObject
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    [RegularExpression("^(Candidate|Company)$", ErrorMessage = "Role must be 'Candidate' or 'Company'.")]
    public string Role { get; set; } = string.Empty;

    // Candidate-specific fields
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }

    // Company-specific fields
    public string? CompanyName { get; set; }
    public string? Description { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var role = Role.ToLower();

        if (role == "candidate")
        {
            if (string.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult("Email is required for candidates.", new[] { nameof(Email) });

            if (string.IsNullOrWhiteSpace(Phone))
                yield return new ValidationResult("Phone is required for candidates.", new[] { nameof(Phone) });

            if (string.IsNullOrWhiteSpace(FullName))
                yield return new ValidationResult("Full Name is required for candidates.", new[] { nameof(FullName) });
        }

        if (role == "company")
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
                yield return new ValidationResult("CompanyName is required for companies.", new[] { nameof(CompanyName) });

            if (string.IsNullOrWhiteSpace(Description))
                yield return new ValidationResult("Description is required for companies.", new[] { nameof(Description) });
        }
    }

}
