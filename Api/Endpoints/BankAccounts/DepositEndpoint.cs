using FluentValidation;
/*
namespace Api.Endpoints.BankAccounts
{
    public class DepositValidator : AbstractValidator<DepositCommand>
    {
        public DepositValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }

    public static class DepositEndpoint
    {
        [EmptyResponse]  // This tells Wolverine that the first "return value" is NOT the response
        [Tags("Accounts")]
        [WolverinePost("/api/accounts/{id}/deposit")]
        public static MoneyDeposited Deposit(Guid id,DepositCommand command, [Aggregate("id")] BankAccount _)
        {
            // return new event to stream
            return new MoneyDeposited(id,command.Amount);
        }
    }
}*/
