using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ZXing.Net.Maui;
#if ANDROID || IOS || MACCATALYST
using ZXing.Net.Maui.Controls;
#endif

namespace Madia.Services;

public class BarcodeScannerService : IBarcodeScannerService
{
#if ANDROID || IOS || MACCATALYST
    public event EventHandler<string>? BarcodeDetected;
#else
    public event EventHandler<string>? BarcodeDetected
    {
        add { }
        remove { }
    }
#endif

#if ANDROID || IOS || MACCATALYST
    CameraBarcodeReaderView? _cameraView;
#endif

    public void Attach(Grid host)
    {
#if ANDROID || IOS || MACCATALYST
        if (_cameraView is not null)
            return;

        _cameraView = new CameraBarcodeReaderView
        {
            IsDetecting = true
        };

        _cameraView.BarcodesDetected += CameraView_BarcodesDetected;
        host.Children.Add(_cameraView);
#endif
    }

    public void Start()
    {
#if ANDROID || IOS || MACCATALYST
        if (_cameraView is not null)
            _cameraView.IsDetecting = true;
#endif
    }

    public void Stop()
    {
#if ANDROID || IOS || MACCATALYST
        if (_cameraView is not null)
            _cameraView.IsDetecting = false;
#endif
    }

    public async Task EnsurePermissionAsync(Page page)
    {
#if ANDROID || IOS || MACCATALYST
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await page.DisplayAlertAsync("Permission", "Camera permission is required to scan barcodes.", "OK");
        }
#else
        await Task.CompletedTask;
#endif
    }

#if ANDROID || IOS || MACCATALYST
    void CameraView_BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results?.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(result))
            return;

        MainThread.BeginInvokeOnMainThread(() => BarcodeDetected?.Invoke(this, result));
    }
#endif
}
