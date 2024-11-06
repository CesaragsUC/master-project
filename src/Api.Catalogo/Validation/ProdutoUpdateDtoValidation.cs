using Api.Catalogo.Dtos;
using FluentValidation;

namespace Api.Catalogo.Validation;

public class ProdutoUpdateDtoValidation : AbstractValidator<ProdutoUpdateDto>
{
    public ProdutoUpdateDtoValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Preco)
            .NotEmpty().WithMessage("Preço é obrigatório")
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");
    }
}
