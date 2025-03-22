using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
/*
namespace Api.Endpoints.BankAccounts
{
    public static class GetBankAccountByIdEndpoint
    {
        [Tags("Accounts")]
        [Produces(typeof(BankAccount))]
        [WolverineGet("/api/accounts/{id}")]
        public static async Task Get(Guid id, IDocumentSession session, HttpContext context)
        {
            await session.Json.WriteById<BankAccount>(id, context);
        }
    }
}
*/