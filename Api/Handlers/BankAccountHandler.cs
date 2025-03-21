using Api.DTOs;
using Api.Entities;
using Api.Events;
using Api.Queries;
using Marten;
using Marten.Events;
using Marten.Pagination;

namespace Api.Handlers
{
    public class BankAccountHandler(IQuerySession querySession)
    {
        public async Task<IEnumerable<BankAccount>> Handle(ListBankAccountsQuery query)
        {
            return await querySession.Query<BankAccount>().ToPagedListAsync(query.Page, query.PageSize);
        }

        public async Task<IEnumerable<BankAccountTransaction>> Handle(AllTransactionsQuery query)
        {
            var events = await querySession.Events.QueryAllRawEvents()
                .ToListAsync();

            return events.Select(x => BankAccountTransaction.Map(x));
        }
        /*
        public async Task<IEnumerable<BankAccountTransaction>> Handle(ListBankAccountTransactionsQuery query)
        {
            IReadOnlyList<IEvent> streamEvents = await querySession.Events.QueryAllRawEvents()
                .Where(x => x.EventTypesAre(typeof(AccountCreated),typeof(MoneyDeposited),typeof(MoneyWithdrawn)) || x.EventTypesAre(typeof(MoneyTransfered)) && x.Data. (x as IEvent<MoneyTransfered>).Data.FromAccountId == query.AccountId)
                .OrderBy(x => x.Sequence)
                .ToListAsync();

            var transferedIEvents = await querySession.Events.QueryRawEventDataOnly<IEvent<MoneyTransfered>>()
                .Where(x => x.Data.ToAccountId == query.AccountId)
                .ToListAsync();

            Console.WriteLine($"IEvent<MoneyTransfered> {transferedIEvents.Count}"); //0

            IReadOnlyList<MoneyTransfered> transferedEvents = await querySession.Events.QueryAllRawEvents()
                .Where(x =>  x.ToAccountId == query.AccountId)
                .ToListAsync();

            Console.WriteLine($"MoneyTransfered {transferedEvents.Count}");  //1



            var events = streamEvents.Union(transferedEvents);



            return streamEvents.Select(x => BankAccountTransaction.Map(x));
        }*/

        // Mutations
        /*
        [Transactional]
        public async Task<BankAccount> Handle(CreateAccountCommand command, CancellationToken ct = default)
        {
            var account = new BankAccount()
            {
                Id = Guid.NewGuid(),
                AccountNumber = "xxx-xxxxxx-x",
                Balance = command.InitialBalance
            };

            session.Store(account);

            await outbox.SendAsync(new AccountCreated(account.Id, account.AccountNumber, account.Balance));

            await session.SaveChangesAsync(ct);

            return account;
        }
        /*
        public async Task Handle(DepositCommand command)
        {
            var account = await session.LoadAsync<BankAccount>(command.AccountId)
                ?? throw new InvalidOperationException("account not found.");

            account.Balance += command.Amount;
            session.Store(account);

            await bus.PublishAsync(new MoneyDeposited(command.AccountId,command.Amount));
        }

        public async Task Handle(WithdrawCommand command)
        {
            var account = await session.LoadAsync<BankAccount>(command.AccountId)
                ?? throw new InvalidOperationException("account not found.");

            if (account.Balance < command.Amount) throw new InvalidOperationException("insufficient funds.");

            account.Balance -= command.Amount;
            session.Store(account);

            await bus.PublishAsync(new MoneyWithdrawn(command.AccountId, command.Amount));
        }

        */
        public static async Task Handle(MoneyTransfered transfered,IDocumentSession session)
        {
            Console.WriteLine($"[transfer] {transfered.FromAccountId} -> {transfered.ToAccountId} : {transfered.Amount}");
            session.Events.Append(transfered.FromAccountId,transfered);
            session.Events.Append(transfered.ToAccountId,transfered);
            await session.SaveChangesAsync();
        }

        public static void Handle(AccountCreated account)
        {
            Console.WriteLine($"## Account Created : {account.AccountId}");
        }
    }
}
