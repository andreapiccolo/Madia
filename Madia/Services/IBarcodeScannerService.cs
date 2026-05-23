using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Madia.Services;

public interface IBarcodeScannerService
{
    event EventHandler<string>? BarcodeDetected;

    void Attach(Grid host);

    void Start();

    void Stop();

    Task EnsurePermissionAsync(Page page);
}
