using Api.Commands;
using Api.DTOs;
using Api.Entities;
using Api.Events;
using Api.Queries;
using Marten;
using Marten.Pagination;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Marten;

namespace Api.Handlers
{
    public class BankAccountHandler(IQuerySession querySession, IDocumentSession session, IMartenOutbox outbox, IMessageBus bus)
    {
        public async Task<IEnumerable<BankAccount>> Handle(ListBankAccountsQuery query)
        {
            return await querySession.Query<BankAccount>().ToPagedListAsync(query.Page, query.PageSize);
        }

        public async Task<IEnumerable<BankAccountTransaction>> Handle(ListBankAccountTransactionsQuery query)
        {
            var events = await querySession.Events.QueryAllRawEvents()
                .Where(x => x.StreamId == query.AccountId)
                .OrderBy(x => x.Sequence)
                .ToListAsync();

            return events.Select(x => BankAccountTransaction.Map(x));
        }

        // Mutations

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

        public async Task Handle(DepositCommand command)
        {
            var account = await session.LoadAsync<BankAccount>(command.AccountId)
                ?? throw new InvalidOperationException("account not found.");

            account.Balance += command.Amount;
            session.Store(account);

            await bus.PublishAsync(new MoneyDeposited(command.Amount));
        }

        public async Task Handle(WithdrawCommand command)
        {
            var account = await session.LoadAsync<BankAccount>(command.AccountId)
                ?? throw new InvalidOperationException("account not found.");

            if (account.Balance < command.Amount) throw new InvalidOperationException("insufficient funds.");

            account.Balance -= command.Amount;
            session.Store(account);

            await bus.PublishAsync(new MoneyWithdrawn( command.Amount));
        }

        public static void Handle(AccountCreated account)
        {
            Console.WriteLine($"## Account Created : {account.AccountId}");
        }
    }
}
