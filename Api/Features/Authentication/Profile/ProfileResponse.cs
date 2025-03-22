namespace Api.Features.Authentication.Profile
{
    public class ProfileResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = [];
    }
}
