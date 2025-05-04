using Api.Const;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.ListAccountTransactions
{
    public class ListAccountTransactionsRequest
    {
        [FromClaim]
        public Guid UserId {  get; set; }

        [RouteParam]
        public Guid AccountId { get; set; }

    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class ListAccountTransactionsEndpoint(IQuerySession session) : Endpoint<ListAccountTransactionsRequest, IEnumerable<Transaction>>
    {
        public override void Configure()
        {
            Get("/accounts/{AccountId}/transactions");
            Roles(Role.All);
        }

        public override async Task HandleAsync(ListAccountTransactionsRequest req, CancellationToken ct)
        {
            var events = await session.Events.FetchStreamAsync(req.AccountId, token: ct);

            if(events is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            List<Transaction> trancsactions = [.. events.Select(x =>
                new Transaction() { Id = x.Id, EventTypeName = x.EventTypeName, Data = x.Data, Timestamp = x.Timestamp })]; 



            await SendAsync(trancsactions,cancellation: ct);
        }
    }
}
