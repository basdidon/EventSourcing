using FluentValidation;

namespace Api.Features.Accounts.Deposit
{
    public class DepositValidator:AbstractValidator<DepositRequest>
    {
        public DepositValidator()
        {
            RuleFor(x=>x.UserId)
                .NotEmpty();

            RuleFor(x=>x.AccountId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
