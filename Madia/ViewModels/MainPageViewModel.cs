using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Madia.Services;

namespace Madia.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    readonly IProductLookupService _productLookupService;
    bool _isProcessing;
    string _resultText = "Point camera at a barcode";
    string _productName = string.Empty;
    string _productBrand = string.Empty;
    string? _productImageUrl;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? RestartRequested;

    public MainPageViewModel(IProductLookupService productLookupService)
    {
        _productLookupService = productLookupService;
        RestartCommand = new Command(RestartScanner);
    }

    public ICommand RestartCommand { get; }

    public string ResultText
    {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }

    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }

    public string ProductBrand
    {
        get => _productBrand;
        set => SetProperty(ref _productBrand, value);
    }

    public string? ProductImageUrl
    {
        get => _productImageUrl;
        set => SetProperty(ref _productImageUrl, value);
    }

    public async Task ProcessBarcodeAsync(string barcode)
    {
        if (_isProcessing)
            return;

        _isProcessing = true;

        try
        {
            ResultText = $"Scanned: {barcode}";

            var lookup = await _productLookupService.LookupByBarcodeAsync(barcode);

            if (!lookup.IsSuccess)
            {
                ProductName = $"Error: {lookup.ErrorMessage}";
                ProductBrand = string.Empty;
                ProductImageUrl = null;
                return;
            }

            if (lookup.IsFound)
            {
                ProductName = lookup.ProductName;
                ProductBrand = lookup.ProductBrand;
                ProductImageUrl = lookup.ImageUrl;
            }
            else
            {
                ProductName = "Product not found";
                ProductBrand = string.Empty;
                ProductImageUrl = null;
            }
        }
        catch (Exception ex)
        {
            ProductName = $"Error: {ex.Message}";
            ProductBrand = string.Empty;
            ProductImageUrl = null;
        }
        finally
        {
            _isProcessing = false;
        }
    }

    void RestartScanner()
    {
        ResultText = "Point camera at a barcode";
        ProductName = string.Empty;
        ProductBrand = string.Empty;
        ProductImageUrl = null;

        RestartRequested?.Invoke();
    }

    bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value))
            return false;

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
