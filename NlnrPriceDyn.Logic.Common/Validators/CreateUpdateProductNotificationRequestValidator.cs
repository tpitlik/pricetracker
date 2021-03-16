using FluentValidation;
using NlnrPriceDyn.Logic.Common.Models;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Validators
{
    public class CreateUpdateProductNotificationRequestValidator : AbstractValidator<CreateUpdateProductNotificationRequest>
    {
        public CreateUpdateProductNotificationRequestValidator()
        {
            RuleFor(x => x.LowPriceLimit).Matches(@"[0-9][0-9,\.]*$").WithMessage(Resources.ErrorIncorrectNumericValue);
        }
    }
}
