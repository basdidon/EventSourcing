using FluentValidation;

namespace Api.Features.Accounts.ListAccounts
{
    public class ListAccountsValidator:AbstractValidator<ListAccountsRequest>
    {
        public ListAccountsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }

    }
}
