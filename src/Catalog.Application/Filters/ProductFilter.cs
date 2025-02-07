using System.Text.Json.Serialization;

namespace Catalog.Application.Filters;

public class ProductFilter
{

    [JsonPropertyName("onlyActive")]
    public bool? OnlyActive { get; set; } = null;

    [JsonPropertyName("name")]
    public string? Name { get; set; } = string.Empty;

    [JsonPropertyName("minPrice")]
    public decimal? MinPrice { get; set; } = null;

    [JsonPropertyName("maxPrice")]
    public decimal? MaxPrice { get; set; } = null;


    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;


    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 10;


    [JsonPropertyName("orderBy")]
    public string? OrderBy { get; set; } = "CreatAt";

    [JsonPropertyName("orderDirection")]
    public string? OrderDirection { get; set; } = "asc";

    public int Skip => (Page - 1) * PageSize;

}

