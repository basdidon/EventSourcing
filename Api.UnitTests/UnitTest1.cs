using Api.Features.Accounts.CreateAccount;
using FakeItEasy;
using FastEndpoints;
using FastEndpoints.Testing;
using Marten;
using FluentAssertions;

namespace Api.UnitTests
{
    class UnitTest1 : TestBase<App>
    {
        [Fact]
        public async void Test1()
        {
            // Arrange
            var ep = Factory.Create<CreateAccountEndpoint>(A.Fake<IDocumentSession>());

            var req = new CreateAccountRequest
            {
                UserId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                InitialBalance = 0,
            };

            // Act
            await ep.HandleAsync(req, default);
            var rsp = ep.Response;

            // Assert
        }
    }
}
