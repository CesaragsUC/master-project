using Api.Catalogo.Dtos;
using FluentValidation;

namespace Api.Catalogo.Validation;

public class ProdutoAddDtoListValidation : AbstractValidator<List<ProdutoAddDto>>
{
    public ProdutoAddDtoListValidation()
    {
        RuleForEach(produto => produto).SetValidator(new ProdutoAddDtoValidation());
    }
}