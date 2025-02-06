using FluentValidation;
using Product.Application.Comands.Product;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Validation;

[ExcludeFromCodeCoverage]
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome do produto não pode ser vazio");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Preço do produto não pode ser vazio");

    }
}
