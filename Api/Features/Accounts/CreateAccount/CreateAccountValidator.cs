using FluentValidation;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0);
        }
    }
}
