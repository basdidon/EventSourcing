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
        public Dictionary<Guid, Account> AccountsBalance { get; set; } = [];
    }

    public class UserAccountsProjection : MultiStreamProjection<UserAccounts, Guid>
    {
        public UserAccountsProjection()
        {
            Identity<UserEvent>(x => x.UserId);
            //Identity<UserRegistered>(x=> x.UserId);
            //Identity<AccountCreated>(x => x.OwnerId);
            //Identity<AccountClosed>(x => x.OwnerId);
            //Identity<MoneyDeposited>(x => x.OwnerId);
            //Identity<MoneyWithdrawn>(x => x.OwnerId);
            Identities<MoneyTransfered>(x => [x.SenderId, x.RecipientId]);
        }

        public UserAccounts Create(UserRegistered e)
        => new()
        {
            UserId = e.UserId
        };

        public void Apply(AccountCreated e, UserAccounts view)
        {
            view.AccountsBalance.Add(
                e.AccountId,
                new Account()
                {
                    Id = e.AccountId,
                    AccountNumber = e.AccountNumber,
                    Balance = e.InitialBalance
                });
        }

        public void Apply(MoneyDeposited e, UserAccounts view)
        {
            view.AccountsBalance[e.AccountId].Balance += e.Amount;
        }

        public void Apply(MoneyWithdrawn e, UserAccounts view)
        {
            view.AccountsBalance[e.AccountId].Balance -= e.Amount;
        }

        public void Apply(MoneyTransfered e, UserAccounts view)
        {
            if (view.UserId == e.SenderId)
            {
                view.AccountsBalance[e.FromAccountId].Balance -= e.Amount;
            }
            else if (view.UserId == e.RecipientId)
            {
                view.AccountsBalance[e.ToAccountId].Balance += e.Amount;
            }
        }

        public void Apply(AccountClosed e, UserAccounts view)
        {
            view.AccountsBalance.Remove(e.AccountId);
        }
    }
}
