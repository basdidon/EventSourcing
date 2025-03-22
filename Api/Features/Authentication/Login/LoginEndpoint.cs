using FastEndpoints;
using FastEndpoints.Security;

namespace Api.Features.Authentication.Login
{
    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        public override void Configure()
        {
            Post("/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var jwtToken = JwtBearer.CreateToken(
                o =>
                {
                    o.ExpireAt = DateTime.UtcNow.AddDays(1);
                    o.User.Roles.Add("Manager", "Auditor");
                    o.User.Claims.Add(("UserName", req.Username));
                    o.User["UserId"] ="B64C2B02-0D4B-420A-825C-CC42C5CA80CA" ; //indexer based claim setting
                });

            await SendAsync(
                new LoginResponse()
                {
                    Token = jwtToken,
                    RefreshToken = "refresh"
                },
                cancellation: ct
            );
        }
    }

}
