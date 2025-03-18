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

        public BankAccount Create(AccountCreated e)
        {
            return new BankAccount()
            {
                Id = e.AccountId,
                AccountNumber = e.AccountNumber,
                Balance = e.InitialBalance
            };
        }

        // No validation here since this event has already occurred. 
        // We are simply updating the state of the aggregate.
        public void Apply(MoneyDeposited e, BankAccount account)
        {
            account.Balance += e.Amount;
        }

        public void Apply(MoneyWithdrawn e, BankAccount account)
        {
            account.Balance -= e.Amount;
        }

        public void Apply(MoneyTransfered e, BankAccount account)
        {
            if(account.Id == e.FromAccountId)
            {
                account.Balance -= e.Amount;
            }
            else if(account.Id == e.ToAccountId)
            {
                account.Balance += e.Amount;
            }
        }
    }
}
