using FastEndpoints;

namespace Api.Features.Users.Profile
{
    public class ProfileResquest
    {
        [FromClaim]
        public Guid UserId { get; set; }
        [FromClaim]
        public string Username { get; set; } = string.Empty;
        [FromClaim]
        public string[] Role { get; set; } = [];
    }
}
