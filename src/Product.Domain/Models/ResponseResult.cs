namespace Product.Domain.Models;

public class ResponseResult<T>
{
    public string? Message { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }
    public long TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string[]? Errors { get; set; }
}