using Api.Events;
using Api.Persistance;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.Withdraw.Confirm
{
    public class WithdrawConfirmEndpoint(ApplicationDbContext context,IDocumentSession session):Endpoint<WithdrawConfirmRequest>
    {
        public override void Configure()
        {
            Post("withdraw/{RequestId}/confirm");
            Roles("Teller","Admin");
        }

        public override async Task HandleAsync(WithdrawConfirmRequest req, CancellationToken ct)
        {
            var withdrawalRequest = await context.Withdrawals.FindAsync([req.RequestId], ct);
            if (withdrawalRequest is null) 
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // ensure request not revocked
            if (withdrawalRequest.IsRevocked)
            {
                AddError("The request has been revoked and cannot be used again.");
                await SendErrorsAsync(410,ct);
                return;
            }

            // reject repeat use request that success
            if (withdrawalRequest.IsSuccess)
            {
                AddError("This request has already been processed and cannot be invoked again.");
                await SendErrorsAsync(410,ct);
                return;
            }

            // expired check
            if(withdrawalRequest.ExpiryDate < DateTime.UtcNow)
            {
                AddError("This request has already been expired.");
                await SendErrorsAsync(410, ct);
            }

            // ensure opt is match
            if(withdrawalRequest.Otp != req.Otp)
            {
                withdrawalRequest.Retry--;
                
                if(withdrawalRequest.Retry <= 0)
                {
                    await RevockWithdrawalRequest(withdrawalRequest, ct);

                    AddError("Maximum OTP retry attempts exceeded. The request has been revoked and cannot be processed.");
                    await SendErrorsAsync(403,ct);
                    return;
                }
                
                await context.SaveChangesAsync(ct);

                AddError("Invalid OTP. Please try again.");
                await SendErrorsAsync(400,ct);
                return;
            }

            // account should exists
            var stream = await session.Events.FetchForWriting<BankAccount>(withdrawalRequest.AccountId,ct);
            var account = stream.Aggregate;
            if(account is null)
            {
                await RevockWithdrawalRequest(withdrawalRequest, ct);

                await SendNotFoundAsync(ct);
                return;
            }

            // ensure sufficient funds
            if (account.Balance < withdrawalRequest.Amount)
            {
                await RevockWithdrawalRequest(withdrawalRequest, ct);

                AddError("insufficient funds.");
                await SendErrorsAsync(400,ct);
            }

            // Process
            stream.AppendOne(new MoneyWithdrawn(withdrawalRequest.AccountId,req.UserId,account.OwnerId,withdrawalRequest.Amount));
            await session.SaveChangesAsync(ct);
        }

        private async Task RevockWithdrawalRequest(WithdrawalRequest request,CancellationToken ct)
        {
            request.IsRevocked = true;
            await context.SaveChangesAsync(ct);
        }

    }
}
