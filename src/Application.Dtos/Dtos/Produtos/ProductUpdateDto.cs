﻿namespace Application.Dtos.Dtos.Produtos;

public record ProductUpdateDto
{
    public string? ProductId { get; set; }
    public string? Name { get; set; }
    public string? ImageUri { get; set; }
    public decimal? Price { get; set; }
    public bool Active { get; set; }
}