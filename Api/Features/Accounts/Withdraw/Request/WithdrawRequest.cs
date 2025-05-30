﻿using FastEndpoints;

namespace Api.Features.Accounts.Withdraw.Request
{
    public class WithdrawRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }  // teller id

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
