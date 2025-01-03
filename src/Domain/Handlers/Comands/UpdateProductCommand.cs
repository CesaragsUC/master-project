﻿using MediatR;
using ResultNet;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Handlers.Comands
{
    public class UpdateProductCommand : IRequest<Result<bool>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [Base64String]
        [JsonPropertyName("imageBase64")]
        public string? ImageBase64 { get; set; }
    }
}
