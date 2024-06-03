using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace Core.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    [StringLength(60)]
    public override string Email { get; set; }
    
    [StringLength(maximumLength:11, MinimumLength = 11)]
    public override string? PhoneNumber { get; set; }
}