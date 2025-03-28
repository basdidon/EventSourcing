/*
namespace Api.Endpoints.BankAccounts
{
    public static class WithdrawEndpoint
    {
        [AggregateHandler]
        public static ProblemDetails Before(WithdrawCommand command,BankAccount bankAccount)
        {
            if(bankAccount.Balance < command.Amount)
            {
                return new ProblemDetails
                {
                    Detail = "Insufficient funds",
                    Status = 428 // precondition required
                };
            }

            return WolverineContinue.NoProblems;
        }

        [EmptyResponse]
        [Tags("Accounts")]
        [WolverinePost("/api/accounts/{id}/withdraw")]
        public static MoneyWithdrawn Withdraw(Guid id,WithdrawCommand command,[Aggregate("id")] BankAccount _)
        {
            return new MoneyWithdrawn(id,command.Amount);
        }
    }
}
*/