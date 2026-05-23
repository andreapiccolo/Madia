using System.Text.Json.Serialization;

namespace MauiBarcodeOpenFood;

public class OpenFoodFactsResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("product")]
    public Product? Product { get; set; }
}

public class Product
{
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("brands")]
    public string? Brands { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}
