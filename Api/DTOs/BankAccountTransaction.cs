using Api.Events;
using Marten.Events;

namespace Api.DTOs
{
    public class BankAccountTransaction
    {
        public Guid TransactionId { get; set; }
        public Guid StreamId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal BalanceChange { get; set; }

        public Guid? SenderId { get; set; }
        public Guid? RecipientId { get; set; }

        public static BankAccountTransaction Map(IEvent @event)
        {
            BankAccountTransaction transaction = new()
            {
                TransactionId = @event.Id,
                StreamId = @event.StreamId,
                TransactionType = @event.EventTypeName
            };

            if (@event.Data is AccountCreated created)
            {
                transaction.BalanceChange = created.InitialBalance;
            }
            else if (@event.Data is MoneyDeposited deposited)
            {
                transaction.BalanceChange = deposited.Amount;
            }
            else if (@event.Data is MoneyWithdrawn withdrawn)
            {
                transaction.BalanceChange = -withdrawn.Amount;
            }
            else if (@event.Data is MoneyTransfered transfered)
            {
                if (transfered.FromAccountId == @event.StreamId)
                {
                    transaction.BalanceChange = -transfered.Amount;
                    transaction.RecipientId = transfered.ToAccountId;
                }
                else if (transfered.ToAccountId == @event.StreamId)
                {
                    transaction.BalanceChange = transfered.Amount;
                    transaction.SenderId = transfered.FromAccountId;
                }
            }


            return transaction;
        }
    }
}
