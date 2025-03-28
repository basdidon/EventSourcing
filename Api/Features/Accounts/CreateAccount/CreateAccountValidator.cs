using FastEndpoints;
using FluentValidation;

namespace Api.Features.Accounts.CreateAccount
{
    public class CreateAccountValidator : Validator<CreateAccountRequest>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CustomerId)
                .NotEmpty();
        }
    }
}
