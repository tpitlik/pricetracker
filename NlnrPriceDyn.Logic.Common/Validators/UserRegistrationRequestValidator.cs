using FluentValidation;
using NlnrPriceDyn.Logic.Common.Models;

namespace NlnrPriceDyn.Logic.Common.Validators
{
    public class UserRegistrationRequestValidator: AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(x => x.UserName).NotNull().WithMessage(Resources.ErrorUserNameBlank);
            RuleFor(x => x.Password).MinimumLength(6).WithMessage(Resources.ErrorPasswordTooShort);
            RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password).WithMessage(Resources.ErrorPasswordsDontMatch);
            RuleFor(x => x.Email).EmailAddress().WithMessage(Resources.ErrorIncorrectEmail);
        }
    }
}
