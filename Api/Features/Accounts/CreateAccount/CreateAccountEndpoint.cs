using Api.Enums;
using Api.Events;
using Api.Features.Accounts.GetAccountById;
using Api.Features.Users;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountEndpoint(IDocumentSession session, UserManager<ApplicationUser> userManager) : Endpoint<CreateAccountRequest, CreateAccountResponse>
    {
        public override void Configure()
        {
            Post("/accounts");
            Roles("Teller", "Admin");
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
        {
            var accountId = Guid.NewGuid();
            var accountNumber = "xxx-xxxxxx-x";

            // ensure CustomerId should exists
            var customer = await userManager.FindByIdAsync(req.CustomerId.ToString());
            if (customer is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Role validate
            var isCustomer = await userManager.IsInRoleAsync(customer, Enums.Roles.Customer.ToString());
            if (!isCustomer)
            {
                AddError(x => x.CustomerId, "Customer with customerId is not in Customer role");
                await SendErrorsAsync(403, ct);
                return;
            }

            session.Events.StartStream(accountId, new AccountCreated(accountId, req.UserId, req.CustomerId, accountNumber, req.InitialBalance));
            await session.SaveChangesAsync(ct);

            // send response
            await SendCreatedAtAsync<GetAccountByIdEndpoint>(
                new // route params
                {
                    AccountId = accountId
                },
                new() // response body
                {
                    AccountId = accountId,
                    AccountNumber = accountNumber
                },
                cancellation: ct);
        }
    }
}
