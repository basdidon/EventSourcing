using Api.Commands;
using Api.Entities;
using Api.Events;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Api.Endpoints.BankAccounts
{


    public static class SendEndpoint
    {
        public static ProblemDetails Before(Guid id, SendCommand command)
        {
            Console.WriteLine("[Before]");
            Console.WriteLine($"From    : {id}");
            Console.WriteLine($"To      : {command.ToAccountId}");
            Console.WriteLine($"Amount  : {command.Amount}");

            if (id == command.ToAccountId)
                return new ProblemDetails() { Status = 400, Detail = "Sender and receiver cannot be the same account." };

            return WolverineContinue.NoProblems;
        }

        public static async Task<ProblemDetails> ValidateAsync(Guid id, BankAccount? sender)
        {
            Console.WriteLine("[ValidateAsync]");
            Console.WriteLine($"route ID    : {id}");
            Console.WriteLine($"senderID    : {sender?.Id}");

            await Task.CompletedTask;

            if (sender == null)
                return new ProblemDetails() { Status = 404, Detail = "Sender's account not found." };

            return WolverineContinue.NoProblems;
        }

        [EmptyResponse]
        [Tags("Accounts")]
        [WolverinePost("/api/accounts/{id}/send")]
        public static MoneyTransfered SendAsync(SendCommand command, [Aggregate("id")] BankAccount sender)
        {
            var @event = new MoneyTransfered(sender.Id, command.ToAccountId, command.Amount);

            return @event;
        }

        [Tags("Accounts")]
        [AlwaysPublishResponse]
        [WolverinePost("/api/v2/accounts/{id}/send")]
        public static MoneyTransfered SendV2(SendCommand command, BankAccount sender)
        {
            var @event = new MoneyTransfered(sender.Id, command.ToAccountId, command.Amount);
            return @event;
        }

        [Tags("Accounts")]
        [WolverinePost("/api/v3/accounts/{id}/send")]
        public static async Task SendV3(Guid id, SendCommand command, IMessageBus bus)
        {
            var @event = new MoneyTransfered(id, command.ToAccountId, command.Amount);
            await bus.PublishAsync(@event);
        }


    }
}
