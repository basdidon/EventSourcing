using FastEndpoints;

namespace Api.Features.Users.Profile
{
    public class ProfileEndpoint : Endpoint<ProfileResquest, ProfileResponse>
    {
        public override void Configure()
        {
            Get("/me");
        }

        public override async Task HandleAsync(ProfileResquest req, CancellationToken ct)
        {
            await SendAsync(
                new ProfileResponse()
                {
                    UserName = req.Username,
                    Roles = req.Role
                },
                cancellation: ct
            );
        }
    }
}
