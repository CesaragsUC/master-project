using Api.Catalogo.Dtos;
using FluentValidation;

namespace Api.Catalogo.Validation;

public class AddProductListDto : AbstractValidator<List<ProductCreateDto>>
{
    public AddProductListDto()
    {
        RuleForEach(produto => produto).SetValidator(new ProductCreateValidator());
    }
}

