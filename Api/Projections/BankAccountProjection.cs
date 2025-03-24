using Api.Entities;
using Api.Events;
using Marten.Events.Aggregation;

namespace Api.Projections
{

    public class BankAccountProjection : SingleStreamProjection<BankAccount>
    {
        public BankAccountProjection()
        {
            DeleteEvent<AccountClosed>();
        }

        public static BankAccount Create(AccountCreated e) => new()
        {
            Id = e.AccountId,
            OwnerId = e.OwnerId,
            AccountNumber = e.AccountNumber,
            Balance = e.InitialBalance
        };

        // No validation here since this event has already occurred. 
        // We are simply updating the state of the aggregate.
        public static void Apply(MoneyDeposited e, BankAccount account) => account.Balance += e.Amount;

        public static void Apply(MoneyWithdrawn e, BankAccount account) => account.Balance -= e.Amount;

        public static void Apply(MoneyTransfered e, BankAccount account)
        {
            if (account.Id == e.FromAccountId)
            {
                account.Balance -= e.Amount;
            }
            else if (account.Id == e.ToAccountId)
            {
                account.Balance += e.Amount;
            }
        }
    }

    /*
    public class BankAccountTransactionsProjection : MultiStreamProjection<BankAccount, Guid>
    {
        public BankAccountTransactionsProjection()
        {
            Identity<AccountCreated>(x => x.AccountId);
            Identity<MoneyDeposited>(x => x.AccountId);
            Identity<MoneyWithdrawn>(x => x.AccountId);
            Identities<IEvent<MoneyTransfered>>(x => [x.Data.FromAccountId, x.Data.ToAccountId]);
        }

        public BankAccount Create(AccountCreated e)
        => new()
        {
            Id = e.AccountId,
            AccountNumber = e.AccountNumber,
            Balance = e.InitialBalance,
        };

        public void Apply(AccountCreated e, BankAccount view)
        {
            view.Id = e.AccountId;
            view.AccountNumber = e.AccountNumber;
            view.Balance = e.InitialBalance;
        }

        public void Apply(MoneyDeposited e, BankAccount view)
        {
            view.Balance += e.Amount;
        }

        public void Apply(MoneyWithdrawn e, BankAccount view)
        {
            view.Balance -= e.Amount;
        }

        public void Apply(MoneyTransfered e, BankAccount view)
        {
            if (view.Id == e.FromAccountId)
            {
                view.Balance -= e.Amount;
            }
            else if (view.Id == e.ToAccountId)
            {
                view.Balance += e.Amount;
            }
        }
    }*/
    /*
    public class TransferredEventGrouper : IAggregateGrouper<Guid>
    {
        public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<Guid> grouping)
        {
            var transferredEvents = events.OfType<IEvent<MoneyTransfered>>()
                .ToList();

            grouping.AddEvents<MoneyTransfered>(x => [x.FromAccountId, x.ToAccountId], transferredEvents);

            await Task.CompletedTask;
        }
    }*/

}
