using Api.DTOs;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Api.Endpoints.BankAccounts
{
    public static class GetBankAccountTransactionsEndpoint
    {
        [Tags("Accounts")]
        [Produces<IEnumerable<BankAccountTransaction>>]
        [WolverineGet("/api/accounts/{id}/transactions")]
        public static async Task<IEnumerable<BankAccountTransaction>> Handle(Guid id, int page, int pageSize, IDocumentSession session)
        {
            var events = await session.Events.QueryAllRawEvents()
                .Where(x => x.StreamId == id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return events.Select(x => BankAccountTransaction.Map(x));
        }
    }
}
