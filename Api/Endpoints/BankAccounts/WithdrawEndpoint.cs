using Api.Commands;
using Api.Entities;
using Api.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

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

        [WolverinePost("/api/accounts/{id}/withdraw")]
        public static MoneyWithdrawn Withdraw(WithdrawCommand command,[Aggregate("id")] BankAccount _)
        {
            return new MoneyWithdrawn(command.Amount);
        }
    }
}
