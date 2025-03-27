using Api.Events;
using Api.Events.User;
using Marten.Events.Projections;

namespace Api.Features.Accounts.ListAccounts
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    public class UserAccounts
    {
        public Guid UserId { get; set; }
        public List<Account> Accounts { get; set; } = [];
    }

    public class UserAccountsProjection : MultiStreamProjection<UserAccounts, Guid>
    {
        public UserAccountsProjection()
        {
            Identity<UserEvent>(x => x.UserId);
            /*Identity<UserRegistered>(x => x.UserId);
            Identity<AccountCreated>(x => x.OwnerId);
            Identity<AccountClosed>(x => x.OwnerId);
            Identity<MoneyDeposited>(x => x.OwnerId);
            Identity<MoneyWithdrawn>(x => x.OwnerId);*/
            Identities<MoneyTransfered>(x => [x.SenderId, x.RecipientId]);
        }

        public void Apply(UserRegistered e, UserAccounts view)
        {
            view.UserId = e.UserId;
        }

        public void Apply(AccountCreated e, UserAccounts view)
        {
            view.Accounts.Add(
                new Account()
                {
                    Id = e.AccountId,
                    AccountNumber = e.AccountNumber,
                    Balance = e.InitialBalance
                });
        }

        public void Apply(MoneyDeposited e, UserAccounts view)
        {
            var account = view.Accounts.Find(x => x.Id == e.AccountId)
                ?? throw new KeyNotFoundException($"account with id {e.AccountId}: notfound");
            account.Balance += e.Amount;
        }

        public void Apply(MoneyWithdrawn e, UserAccounts view)
        {
            var account = view.Accounts.Find(x => x.Id == e.AccountId)
                ?? throw new KeyNotFoundException($"account with id {e.AccountId}: notfound");
            account.Balance -= e.Amount;
        }

        public void Apply(MoneyTransfered e, UserAccounts view)
        {


            if (view.UserId == e.SenderId)
            {
                var fromAccount = view.Accounts.Find(x => x.Id == e.FromAccountId)
                    ?? throw new KeyNotFoundException($"account with id {e.FromAccountId}: notfound");
                fromAccount.Balance -= e.Amount;
            }
            else if (view.UserId == e.RecipientId)
            {
                var toAccount = view.Accounts.Find(x => x.Id == e.ToAccountId)
                    ?? throw new KeyNotFoundException($"account with id {e.ToAccountId}: notfound");
                toAccount.Balance += e.Amount;
            }
        }

        public void Apply(AccountClosed e, UserAccounts view)
        {
            var toDelete = view.Accounts.Find(x => x.Id == e.AccountId);
            if (toDelete != null) {
                view.Accounts.Remove(toDelete);
            }
        }
    }
}
