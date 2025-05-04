using FluentValidation;

namespace Api.Features.Accounts.FreezeAccount
{
    public class FreezeAccountValidator : AbstractValidator<FreezeAccountRequest>
    {
        public FreezeAccountValidator()
        {
            RuleFor(x => x.AdminId)
                .NotEmpty()
                .WithMessage("AdminId cant be empty.");

            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("AccountId cant be empty.");

        }
    }
}
