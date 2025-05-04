using Api.Events;
using Marten.Events.Aggregation;

namespace Api.Features.Accounts
{

    public class BankAccountProjection : SingleStreamProjection<BankAccount>
    {
        public BankAccountProjection()
        {
            DeleteEvent<AccountClosed>();
        }

        public static BankAccount Create(AccountCreated e) => new()
        {
            OwnerId = e.OwnerId,
            AccountNumber = e.AccountNumber,
            Balance = e.InitialBalance,
            IsFrozen = false,
        };

        public static void Apply(AccountFrozen e, BankAccount account) => account.IsFrozen = true;
        public static void Apply(AccountUnfrozen e, BankAccount account) => account.IsFrozen = false;

        // No validation here since this event has already occurred. 
        // We are simply updating the state of the aggregate.
        public static void Apply(MoneyDeposited e, BankAccount account) => account.Balance += e.Amount;

        public static void Apply(MoneyWithdrawn e, BankAccount account) => account.Balance -= e.Amount;

        public static void Apply(MoneySent e,BankAccount account) => account.Balance -= e.Amount;
        public static void Apply(MoneyReceived e, BankAccount account) => account.Balance += e.Amount;
    }
}
