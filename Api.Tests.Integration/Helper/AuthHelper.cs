using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Api.Tests.Integration.Helper
{
    internal class AuthHelper
    {
        public static async Task LoginAsync(HttpClient client, string username, string password)
        {
            var request = "/api/v1/login";
            var loginRequest = new { username, password };

            //var response = await client.PostAsJsonAsync(request,loginRequest);
            var response = await client.PostAsync(request,
               new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(jsonString);
            var token = json.GetProperty("accessToken").GetString() ?? string.Empty;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
