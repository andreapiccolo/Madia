using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Dispatching;
using ZXing.Net.Maui;

namespace Madia;

public partial class MainPage : ContentPage
{
    readonly HttpClient _http = new HttpClient();
    bool _isProcessing;

    public MainPage()
    {
        InitializeComponent();
        RequestCameraPermissionAsync();
    }

    async Task RequestCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission", "Camera permission is required to scan barcodes.", "OK");
        }
    }

    void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;

        var result = e.Results?.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(result)) return;

        _isProcessing = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            lblResult.Text = $"Scanned: {result}";
            cameraView.IsDetecting = false;
            await LookupProductAsync(result);
            _isProcessing = false;
        });
    }

    async Task LookupProductAsync(string barcode)
    {
        try
        {
            var url = $"https://world.openfoodfacts.net/api/v3/product/{barcode}?fields=product_name,brands,quantity,image_url";
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("Madia/1.0 (andreapiccolo)");

            var resp = await _http.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
            {
                lblName.Text = "Product not found";
                lblBrand.Text = string.Empty;
                productImage.Source = null;
                return;
            }

            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenFoodFactsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data?.Status == 1 && data.Product != null)
            {
                lblName.Text = data.Product.ProductName ?? "—";
                lblBrand.Text = data.Product.Brands ?? "—";
                if (!string.IsNullOrEmpty(data.Product.ImageUrl))
                    productImage.Source = ImageSource.FromUri(new Uri(data.Product.ImageUrl));
                else
                    productImage.Source = null;
            }
            else
            {
                lblName.Text = "Product not found";
                lblBrand.Text = string.Empty;
                productImage.Source = null;
            }
        }
        catch (Exception ex)
        {
            lblName.Text = $"Error: {ex.Message}";
            lblBrand.Text = string.Empty;
            productImage.Source = null;
        }
    }

    void OnRestartClicked(object sender, EventArgs e)
    {
        lblResult.Text = "Point camera at a barcode";
        lblName.Text = string.Empty;
        lblBrand.Text = string.Empty;
        productImage.Source = null;
        cameraView.IsDetecting = true;
    }
}
