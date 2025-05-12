using Api.Tests.Integration.Tests.Abstract;

namespace Api.Tests.Integration.Tests
{
    [Collection(nameof(DatabaseTestCollection))]
    public class ListAccountsApiTest(IntegrationTestFactory factory) : BaseApiTests(factory)
    {
        const string requestEndpoint = "/api/v1/accounts";

        [Fact]
        public async Task List_Account_With_No_QueryParams_Should_Success()
        {
            await SeedDb();
            // Arrange

            // Act
            await LoginBySeedUserAsync("customer01");

            var res = await client.GetAsync(requestEndpoint);
            
            // Assert
            res.EnsureSuccessStatusCode();
        }
    }
}
