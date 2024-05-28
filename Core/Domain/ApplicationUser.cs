using Microsoft.AspNetCore.Identity;

namespace Core.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? PersonName { get; set; }
}