using System;
using Microsoft.Maui.Controls;
using Madia.Services;
using Madia.ViewModels;

namespace Madia;

public partial class MainPage : ContentPage
{
    readonly MainPageViewModel _viewModel;
    readonly IBarcodeScannerService _scannerService;

    public MainPage(MainPageViewModel viewModel, IBarcodeScannerService scannerService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _scannerService = scannerService;
        BindingContext = _viewModel;

        _viewModel.RestartRequested += HandleRestartRequested;
        _scannerService.BarcodeDetected += HandleBarcodeDetected;

        _scannerService.Attach(scannerHost);
        _ = _scannerService.EnsurePermissionAsync(this);
    }

    async void HandleBarcodeDetected(object? sender, string barcode)
    {
        _scannerService.Stop();
        await _viewModel.ProcessBarcodeAsync(barcode);
    }

    void HandleRestartRequested()
    {
        _scannerService.Start();
    }

    protected override void OnDisappearing()
    {
        _viewModel.RestartRequested -= HandleRestartRequested;
        _scannerService.BarcodeDetected -= HandleBarcodeDetected;
        base.OnDisappearing();
    }
}
