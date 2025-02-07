﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Application.Dtos.Dtos.Login;

[ExcludeFromCodeCoverage]
public class RealmAccess
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; }
}
