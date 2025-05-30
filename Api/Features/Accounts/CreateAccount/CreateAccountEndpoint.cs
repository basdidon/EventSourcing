﻿using Api.Const;
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
            Roles(Role.Teller,Role.Admin);
            // default authentication scheme return NotFound(404) when unauthorize(401) and forbidden(403)
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
        {
            var accountNumber = "xxx-xxxxxx-x";

            // ensure CustomerId should exists
            var customer = await userManager.FindByIdAsync(req.CustomerId.ToString());
            if (customer is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Role validate
            var isCustomer = await userManager.IsInRoleAsync(customer, Role.Customer);
            if (!isCustomer)
            {
                AddError(x => x.CustomerId, "Customer with customerId is not in Customer role");
                await SendErrorsAsync(400, ct);
                return;
            }

            var accountId = session.Events.StartStream(new AccountCreated(req.CustomerId, req.UserId, accountNumber, req.InitialBalance)).Id;
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
