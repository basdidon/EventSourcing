using Api.Persistance;
using FastEndpoints.Security;

namespace Api.Features.Users.Auth.RefreshToken
{
    public class BlacklistChecker(RequestDelegate next,ILogger<BlacklistChecker> logger) : JwtRevocationMiddleware(next)
    {
        protected override Task<bool> JwtTokenIsValidAsync(string jwtToken, CancellationToken ct)
        {
            logger.LogInformation("jwtToken : {}",jwtToken);
            return Task.FromResult(true);
        }
    }
}
