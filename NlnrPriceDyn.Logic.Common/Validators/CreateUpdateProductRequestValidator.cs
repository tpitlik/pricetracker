using FluentValidation;
using NlnrPriceDyn.Logic.Common.Models.ProductManagement;

namespace NlnrPriceDyn.Logic.Common.Validators
{
    public class CreateUpdateProductRequestValidator: AbstractValidator<CreateUpdateProductRequest>
    {
        public CreateUpdateProductRequestValidator()
        {
            RuleFor(x => x.LinkUrl).NotEmpty().WithMessage(Resources.ErrorProductUrlBlank);
            RuleFor(x => x.Title).NotEmpty().WithMessage(Resources.ErrorProductTitleBlank);
            //RuleFor(x => x.UserId).NotEmpty().WithMessage(Resources.ErrorUserIdBlank);
        }
    }
}
