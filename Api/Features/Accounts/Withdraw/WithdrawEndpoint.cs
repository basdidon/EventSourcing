using Api.Entities;
using Api.Persistance;
using Api.Services;
using FastEndpoints;
using Marten;

namespace Api.Features.Accounts.Withdraw
{
    public class WithdrawEndpoint(IDocumentSession session, ApplicationDbContext context, OtpService otpService, SmsService smsService) : Endpoint<WithdrawRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{AccountId}/withdraw");
            Roles("teller", "admin");
        }

        public override async Task HandleAsync(WithdrawRequest req, CancellationToken ct)
        {
            /// TODO:
            /// - ensure account is exists
            /// - ensure account balance is enough
            /// - generate opt
            /// - create WithdrawRequest
            /// - send opt to account's owner

            var account = await session.LoadAsync<BankAccount>(req.AccountId, ct);
            if (account is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (account.Balance < req.Amount)
            {
                AddError(x => x.Amount, "insufficient funds.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            string optCode = otpService.GenerateOTP();
            var stream = await session.Events.FetchForWriting<BankAccount>(req.AccountId, ct);


            WithdrawalRequest request = new()
            {
                AccountId = req.AccountId,
                Amount = req.Amount,
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                Otp = optCode,
                Retry = 5,
            };

            await context.Withdrawals.AddAsync(request, ct);
            await context.SaveChangesAsync(ct);

            await smsService.SendOtpAsync("xxx-xxx-xxxx", optCode);
        }
    }
}