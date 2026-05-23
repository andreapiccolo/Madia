using System.Threading.Tasks;

namespace Madia.Services;

public interface IProductLookupService
{
    Task<ProductLookupResult> LookupByBarcodeAsync(string barcode);
}

public sealed class ProductLookupResult
{
    public bool IsSuccess { get; init; }

    public bool IsFound { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public string ProductBrand { get; init; } = string.Empty;

    public string? ImageUrl { get; init; }

    public string? ErrorMessage { get; init; }
}
