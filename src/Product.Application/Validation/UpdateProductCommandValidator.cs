using FluentValidation;
using Product.Application.Comands.Product;

namespace Product.Application.Validation;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id do produto não pode ser vazio");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome do produto não pode ser vazio");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Preço do produto não pode ser vazio");

    }
}
