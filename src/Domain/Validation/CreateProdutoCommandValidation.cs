﻿using Domain.Handlers.Comands;
using FluentValidation;

namespace Domain.Validation
{
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
}
