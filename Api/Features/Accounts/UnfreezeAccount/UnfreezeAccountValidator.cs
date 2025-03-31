using FluentValidation;

namespace Api.Features.Accounts.UnfreezeAccount
{
    public class UnfreezeAccountValidator: AbstractValidator<UnfreezeAccountRequest>
    {
        public UnfreezeAccountValidator()
        {
            RuleFor(x => x.AdminId)
                .NotEmpty();

            RuleFor(x => x.AccountId)
                .NotEmpty();
        }
    }
}
