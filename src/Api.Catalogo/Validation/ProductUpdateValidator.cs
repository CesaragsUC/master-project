using Api.Catalogo.Dtos;
using FluentValidation;

namespace Api.Catalogo.Validation;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Preco)
            .NotEmpty().WithMessage("Preço é obrigatório")
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");
    }
}

