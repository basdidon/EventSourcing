using FluentValidation;

namespace Api.Features.Accounts.FreezeAccount
{
    public class FreezeAccountValidator : AbstractValidator<FreezeAccountRequest>
    {
        public FreezeAccountValidator()
        {
            RuleFor(x => x.AdminId)
                .NotEmpty();

            RuleFor(x => x.AccountId)
                .NotEmpty();
        }
    }
}
