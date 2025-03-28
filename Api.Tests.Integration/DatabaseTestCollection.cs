namespace Api.Tests.Integration
{
    [CollectionDefinition(nameof(DatabaseTestCollection))]
    public class DatabaseTestCollection : ICollectionFixture<IntegrationTestFactory>
    {
    }
}
