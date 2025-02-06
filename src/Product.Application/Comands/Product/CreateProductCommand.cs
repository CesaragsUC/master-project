
using MediatR;
using System.ComponentModel.DataAnnotations;
using ResultNet;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Comands.Product;

[ExcludeFromCodeCoverage]
public class CreateProductCommand : IRequest<Result<bool>>
{
    public string? Name { get; set; }

    public decimal Price { get; set; }

    public bool Active { get; set; }

    [Base64String]
    public string? ImageBase64 { get; set; }

}
