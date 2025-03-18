using Api.Entities;
using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Api.Endpoints.BankAccounts
{
    public static class GetBankAccountByIdEndpoint
    {
        [Produces(typeof(BankAccount))]
        [WolverineGet("/stream/accounts/{id}")]
        public static async Task Get(Guid id, IDocumentSession session, HttpContext context)
        {
            await session.Json.WriteById<BankAccount>(id, context);
        }
    }
}
