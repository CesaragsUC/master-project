using Domain.Handlers.Comands;
using FluentValidation;

namespace Domain.Validation
{
    public class CriarProdutoCommandValidation : AbstractValidator<CreateProdutoCommand>
    {
        public CriarProdutoCommandValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome do produto não pode ser vazio");

            RuleFor(x => x.Preco)
                .NotEmpty()
                .WithMessage("Preço do produto não pode ser vazio");

            RuleFor(x => x.Active)
                .NotEmpty()
                .WithMessage("Active do produto não pode ser vazio");

        }
    }
}
