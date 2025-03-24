using Api.Features.Users.Auth.RefreshToken;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Api.Features.Users.Auth.Login
{
    public class LoginEndpoint(UserManager<ApplicationUser> userManager) : Endpoint<LoginRequest, TokenResponse>
    {
        public override void Configure()
        {
            Post("/login");
            AllowAnonymous();
            Description(x => x.AutoTagOverride("User"));
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await userManager.FindByNameAsync(req.Username);

            if (user is not null && await userManager.CheckPasswordAsync(user, req.Password))
            {
                var roles = await userManager.GetRolesAsync(user);

                Response = await CreateTokenWith<FastEndpointsTokenService>(user.Id.ToString(), u =>
                {
                    u.Roles.AddRange(roles);
                    u.Claims.Add(new Claim("UserId", user.Id.ToString()));
                });
            }
            else
            {
                await SendUnauthorizedAsync(ct);
            }
        }
    }

}
