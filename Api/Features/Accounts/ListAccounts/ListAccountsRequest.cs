using FastEndpoints;
using System.ComponentModel;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsRequest
    {
        [FromClaim]
        public Guid UserId { get; set; }

        public Guid? OwnerId { get; set; }

        [DefaultValue(1)] // for swagger
        public int Page { get; set; } = 1;
        
        [DefaultValue(20)] // for swagger
        public int PageSize { get; set; } = 20;
    }
}
