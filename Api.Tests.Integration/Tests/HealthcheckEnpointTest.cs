namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class HealthcheckEnpointTest(IntegrationTestFactory factory)
    {
        protected readonly HttpClient client = factory.CreateClient();

        [Fact]
        public async Task HealthCheck()
        {
            string request = "/health";
            var response = await client.GetAsync(request);

            response.EnsureSuccessStatusCode();
        }
    }
}