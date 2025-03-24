using Api.Persistance;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace Api.Features.Users.Auth.RefreshToken
{
    public class FastEndpointsTokenService : RefreshTokenService<TokenRequest, TokenResponse>
    {
        private ApplicationDbContext DbContext { get; }
        private UserManager<ApplicationUser> UserManager { get; }

        public FastEndpointsTokenService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            DbContext = context;
            UserManager = userManager;

            Setup(x =>
            {
                x.AccessTokenValidity = TimeSpan.FromMinutes(1);
                x.RefreshTokenValidity = TimeSpan.FromHours(1);
                x.Endpoint("/user/auth/refresh-token", ep =>
                {
                    ep.Summary(s => s.Description = "this is the refresh token endpoint");
                });
            });
        }

        public override async Task PersistTokenAsync(TokenResponse response)
        {
            if (!Guid.TryParse(response.UserId, out Guid userIdAsGuid))
                throw new ArgumentException("user id is not GUID");

            await DbContext.RefreshTokens.AddAsync(new()
            {
                Token = response.RefreshToken,
                UserId = userIdAsGuid,
                ExpiryTime = response.RefreshExpiry,
            });
            await DbContext.SaveChangesAsync();
        }

        public override async Task RefreshRequestValidationAsync(TokenRequest req)
        {
            var refreshToken = await DbContext.RefreshTokens.FindAsync(req.RefreshToken);
            if (refreshToken is null)
                AddError(r => r.RefreshToken, "Refresh token is invalid!");
        }

        public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
        {
            var user = await UserManager.FindByIdAsync(request.UserId)
                ?? throw new Exception($"user with id {request.UserId} is notfound");

            var roles = await UserManager.GetRolesAsync(user);
            privileges.Claims.Add(new("UserId", user.Id.ToString()));
            privileges.Roles.AddRange(roles);
        }
    }
}
