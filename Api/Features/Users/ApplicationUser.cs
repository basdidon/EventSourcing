using Microsoft.AspNetCore.Identity;

namespace Api.Features.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
    }

    public class ApplicationRole : IdentityRole<Guid> { }
}
