namespace Application.Dtos.Dtos.Login;

public record Claims
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}