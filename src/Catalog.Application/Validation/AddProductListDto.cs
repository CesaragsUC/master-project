using Application.Dtos.Dtos.Produtos;
using FluentValidation;

namespace Catalog.Application.Validation;

public class AddProductListDto : AbstractValidator<List<ProductCreateDto>>
{
    public AddProductListDto()
    {
        RuleForEach(produto => produto).SetValidator(new ProductCreateValidator());
    }
}

