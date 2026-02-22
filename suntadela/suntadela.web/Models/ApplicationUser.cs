using Microsoft.AspNetCore.Identity;

namespace suntadela.web.Models;

public class ApplicationUser : IdentityUser
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
