﻿using Api.Const;
using Api.Events;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Features.Accounts.Send
{

    public class SendEndpoint(IDocumentSession session) : Endpoint<SendRequest>
    {
        public override void Configure()
        {
            Post("/accounts/{FromAccountId}/send");
            Roles(Role.Customer);
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(SendRequest req, CancellationToken ct)
        {
            // ensure both accounts existing
            var fromStream = await session.Events.FetchForWriting<BankAccount>(req.FromAccountId, ct);
            var fromAccount = fromStream.Aggregate;
            if (fromAccount is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (fromAccount.OwnerId != req.UserId)
            {
                await SendErrorsAsync(400, ct);
            }

            if (fromAccount.Balance < req.Amount)
            {
                await SendErrorsAsync(400, ct);
            }

            if (fromAccount.IsFrozen)
            {
                AddError("fromAccount is frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            var toStream = await session.Events.FetchForWriting<BankAccount>(req.ToAccountId, ct);
            var toAccount = toStream.Aggregate;
            if (toAccount is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (toAccount.IsFrozen)
            {
                AddError("toAccount is frozen.");
                await SendErrorsAsync(409, ct);
                return;
            }

            // process
            Guid transactionId = Guid.NewGuid();
            MoneySent moneySent = new(transactionId, req.Amount);
            MoneyReceived moneyReceived = new(transactionId, req.Amount);

            fromStream.AppendOne(moneySent);
            toStream.AppendOne(moneyReceived);

            await session.SaveChangesAsync(ct);
        }
    }
}
