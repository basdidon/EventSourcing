using FluentValidation;

namespace Api.Features.Accounts.Withdraw.Confirm
{
    public class WithdrawConfirmValidator : AbstractValidator<WithdrawConfirmRequest>
    {
        public WithdrawConfirmValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.Otp)
                .Matches(@"^\d{6}$")
                .Length(6);
        }
    }
}
