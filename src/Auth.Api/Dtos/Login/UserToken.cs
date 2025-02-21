﻿using System.Diagnostics.CodeAnalysis;

namespace Auth.Api.Dtos.Login;

[ExcludeFromCodeCoverage]
public record UserToken
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public IEnumerable<Claims>? Claims { get; set; }
    public IEnumerable<string>? Groups { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
