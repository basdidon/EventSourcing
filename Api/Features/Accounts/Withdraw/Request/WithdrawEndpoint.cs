using Api.Const;
using Api.Events;
using Api.Persistance;
using Api.Services;
using FastEndpoints;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.Withdraw.Request
{
    public class WithdrawEndpoint(IDocumentSession session, OtpService otpService, SmsService smsService) : Endpoint<WithdrawRequest,WithdrawResponse>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/withdraw");
            Roles(Role.Teller,Role.Admin);
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(WithdrawRequest req, CancellationToken ct)
        {
            var account = await session.LoadAsync<BankAccount>(req.AccountId, ct);
            if (account is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (account.IsFrozen)
            {
                AddError("account is frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            if (account.Balance < req.Amount)
            {
                AddError(x => x.Amount, "insufficient funds.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            var toRevockIds = session.Query<Withdrawal>()
                .Where(x =>
                    x.AccountId == req.AccountId && x.IsSuccess == false && x.IsRevocked == false)
                .Select(x => x.RequestId);

            foreach(var toRevockId in toRevockIds)
            {
                session.Events.Append(toRevockId, new WithdrawRevocked(),new Archived("Revoked due to a new withdrawal request by the user"));
            }


            string otpCode = otpService.GenerateOTP();

            WithdrawRequested requested = new(req.AccountId,req.Amount,otpCode,DateTimeOffset.UtcNow.AddMinutes(15),5,req.UserId);
            var requestId = session.Events.StartStream(requested).Id;
            await session.SaveChangesAsync(ct);

            await smsService.SendOtpAsync("xxx-xxx-xxxx", otpCode);

            await SendAsync(new WithdrawResponse() { RequestId = requestId },cancellation: ct);
        }
    }
}