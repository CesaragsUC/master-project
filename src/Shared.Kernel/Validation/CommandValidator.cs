using FluentValidation;
using Shared.Kernel.Common;

namespace Shared.Kernel.Validation;

public abstract class CommandValidator
{
    public async Task<ResponseResult<bool>> Validator<T>(T obj, AbstractValidator<T> validator)
    {
        var validationResult = validator.Validate(obj);
        var result = new ResponseResult<bool>();

        if (!validationResult.IsValid)
        {
            result.Success = false;
            result.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return result;
        }

        result.Success = true;
        return result;
    }
}
