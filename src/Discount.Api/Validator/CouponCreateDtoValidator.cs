using Discount.Domain.Dtos;
using FluentValidation;

namespace Discount.Api.Validator;

public class CouponCreateDtoValidator : AbstractValidator<CouponCreateDto>
{
    public CouponCreateDtoValidator()
    {
        RuleFor(x => x.Code).MinimumLength(8).NotEmpty().NotNull().WithMessage("Code is required");
        RuleFor(x => x.Type).IsInEnum().NotNull().WithMessage("Type is required");
        RuleFor(x => x.Value).GreaterThan(0).WithMessage("Value should be greater than 0");
        RuleFor(x => x.MinValue).GreaterThan(0).WithMessage("Min Value should be greater than 0");
        RuleFor(x => x.StartDate).NotNull().NotEmpty().WithMessage("Start Date is required");
        RuleFor(x => x.EndDate).NotNull().NotEmpty().WithMessage("End Date is required");
        RuleFor(x => x.Active).NotNull().WithMessage("Active is required");
        RuleFor(x => x.MaxUse).GreaterThan(0).WithMessage("MaxUse Value should be greater than 0");
    }
}
