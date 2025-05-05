using Api.Const;
using Api.Events;
using FastEndpoints;
using FastEndpoints.Swagger;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.Withdraw.Confirm
{
    public class WithdrawConfirmEndpoint(IDocumentSession session):Endpoint<WithdrawConfirmRequest>
    {
        public override void Configure()
        {
            Post("withdraw/{RequestId}/confirm");
            Description(o => o.AutoTagOverride("accounts"));
            Roles(Role.Teller,Role.Admin);
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(WithdrawConfirmRequest req, CancellationToken ct)
        {
            var withdrawalStream = await session.Events.FetchForWriting<Withdrawal>(req.RequestId, ct);
            var withdrawal = withdrawalStream.Aggregate; 
            
            if(withdrawalStream is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // ensure request not revocked
            if (withdrawal.IsRevocked)
            {
                AddError("The request has been revoked and cannot be used again.");
                await SendErrorsAsync(410,ct);
                return;
            }

            // reject repeat use request that success
            if (withdrawal.IsSuccess)
            {
                AddError("This request has already been processed and cannot be invoked again.");
                await SendErrorsAsync(410,ct);
                return;
            }

            // expired check
            if(withdrawal.ExpiryDate < DateTimeOffset.UtcNow)
            {
                AddError("This request has already been expired.");
                await SendErrorsAsync(410, ct);
            }

            // ensure opt is match
            if(withdrawal.Otp != req.Otp)
            {
                withdrawalStream.AppendOne(new WithdrawRejected());
                
                if(withdrawal.Retry <= 1)
                {
                    withdrawalStream.AppendOne(new WithdrawRevocked());
                    withdrawalStream.AppendOne(new Archived("Maximum OTP retry attempts exceeded."));
                    await session.SaveChangesAsync(ct);

                    AddError("Maximum OTP retry attempts exceeded. The request has been revoked and cannot be processed.");
                    await SendErrorsAsync(403,ct);
                    return;
                }

                await session.SaveChangesAsync(ct);

                AddError("Invalid OTP. Please try again.");
                await SendErrorsAsync(400,ct);
                return;
            }

            // account should exists
            var accountStream = await session.Events.FetchForWriting<BankAccount>(withdrawal.AccountId,ct);
            var account = accountStream.Aggregate;
            if(account is null)
            {
                withdrawalStream.AppendOne(new WithdrawRevocked());
                withdrawalStream.AppendOne(new Archived("Account notfound."));
                await session.SaveChangesAsync(ct);

                await SendNotFoundAsync(ct);
                return;
            }

            if (account.IsFrozen)
            {
                withdrawalStream.AppendOne(new WithdrawRevocked());
                withdrawalStream.AppendOne(new Archived("account is frozen."));
                await session.SaveChangesAsync(ct);

                AddError("account is frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            // ensure sufficient funds
            if (account.Balance < withdrawal.Amount)
            {
                withdrawalStream.AppendOne(new WithdrawRevocked());
                withdrawalStream.AppendOne(new Archived("insufficient funds."));
                await session.SaveChangesAsync(ct);

                AddError("insufficient funds.");
                await SendErrorsAsync(400,ct);
            }

            // Process
            withdrawalStream.AppendOne(new WithdrawConfirmed());
            withdrawalStream.AppendOne(new Archived("successful."));
            accountStream.AppendOne(new MoneyWithdrawn(withdrawal.Amount,req.UserId));
            await session.SaveChangesAsync(ct);
        }
    }
}
