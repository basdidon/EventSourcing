using FluentValidation;

namespace Api.Features.Accounts.Send
{
    public class SendValidator : AbstractValidator<SendRequest>
    {
        public SendValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.FromAccountId)
                .NotEmpty();

            RuleFor(x => x.ToAccountId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }

}
