using Api.Commands;
using Api.Entities;
using Api.Events;
using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Api.Endpoints.BankAccounts
{
    public class CreateBankAccountValidator: AbstractValidator<CreateAccountCommand>
    {
        public CreateBankAccountValidator()
        {
            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0);
        }
    }

    public static class CreateBankAccountEndpoint
    {
        [WolverinePost("/api/accounts")]
        public static (CreationResponse<Guid>,IStartStream) Create(CreateAccountCommand command)
        {
            var createEvent = new AccountCreated(Guid.NewGuid(), "xxx-xxxxxx-x", command.InitialBalance);
            var start = MartenOps.StartStream<BankAccount>(createEvent);

            var response = new CreationResponse<Guid>("/api/v2/accounts/" + start.StreamId, start.StreamId);

            return (response, start);
        }
    }
}
