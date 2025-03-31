using FluentValidation;

namespace Api.Features.Accounts.GetAccountById
{
    public class GetAccountByIdValidator : AbstractValidator<GetAccountByIdRequest>
    {
        public GetAccountByIdValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.AccountId)
                .NotEmpty();
        }
    }
}
