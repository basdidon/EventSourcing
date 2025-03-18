using Api.Commands;
using Api.Entities;
using Api.Events;
using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Api.Endpoints
{
    public static class AccountEndpoint
    {
        [WolverinePost("/stream/accounts")]
        public static (CreationResponse<Guid>, IStartStream) Post(CreateAccountCommand command)
        {
            var createEvent = new AccountCreated(Guid.NewGuid(), "xxx-xxxxxx-x", command.InitialBalance);
            var start = MartenOps.StartStream<BankAccount>(createEvent);

            var response = new CreationResponse<Guid>("/stream/accounts/" + start.StreamId, start.StreamId);

            return (response, start);
        }

        [EmptyResponse]  // This tells Wolverine that the first "return value" is NOT the response
        [WolverinePost("/stream/accounts/{id}/deposit")]
        public static MoneyDeposited Deposit(DepositCommand command, [Aggregate("id")] BankAccount account)
        {
            // return new event to stream
            return new MoneyDeposited(command.Amount);
        }

        [Produces(typeof(BankAccount))]
        [WolverineGet("/stream/accounts/{id}")]
        public static async Task Get(Guid id,IDocumentSession session,HttpContext context)
        {
            await session.Json.WriteById<BankAccount>(id, context);
        }
    }
}
