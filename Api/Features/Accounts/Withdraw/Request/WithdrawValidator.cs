using FluentValidation;

namespace Api.Features.Accounts.Withdraw.Request
{
    public class WithdrawValidator:AbstractValidator<WithdrawRequest>
    {
        public WithdrawValidator()
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
