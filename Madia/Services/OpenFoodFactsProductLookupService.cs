using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Madia.Services;

public sealed class OpenFoodFactsProductLookupService : IProductLookupService
{
    readonly HttpClient _http = new HttpClient();

    public OpenFoodFactsProductLookupService()
    {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("Madia/1.0 (andreapiccolo)");
    }

    public async Task<ProductLookupResult> LookupByBarcodeAsync(string barcode)
    {
        try
        {
            var url = $"https://world.openfoodfacts.net/api/v3/product/{barcode}?fields=product_name,brands,quantity,image_url";
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new ProductLookupResult
                {
                    IsSuccess = true,
                    IsFound = false
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenFoodFactsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data?.Status == 1 && data.Product is not null)
            {
                return new ProductLookupResult
                {
                    IsSuccess = true,
                    IsFound = true,
                    ProductName = data.Product.ProductName ?? "-",
                    ProductBrand = data.Product.Brands ?? "-",
                    ImageUrl = data.Product.ImageUrl
                };
            }

            return new ProductLookupResult
            {
                IsSuccess = true,
                IsFound = false
            };
        }
        catch (Exception ex)
        {
            return new ProductLookupResult
            {
                IsSuccess = false,
                IsFound = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
