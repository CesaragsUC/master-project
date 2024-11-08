using Application.Dtos.Dtos.Produtos;
using FluentValidation;

namespace Catalog.Service.Validation;

public class AddProductListDto : AbstractValidator<List<ProductCreateDto>>
{
    public AddProductListDto()
    {
        RuleForEach(produto => produto).SetValidator(new ProductCreateValidator());
    }
}

