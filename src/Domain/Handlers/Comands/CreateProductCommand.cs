using Application.Dtos.Dtos.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Domain.Handlers.Comands
{
    public class CreateProductCommand : IRequest<Result<bool>>
    {
        public string? Name { get; set; }

        public decimal Price { get; set; }

        public bool Active { get; set; }

        [Base64String]
        public string? ImageBase64 { get; set; }

    }
}
