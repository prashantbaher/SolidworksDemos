using System.Runtime.InteropServices;

namespace SolidWorksApiLib;

public class SolidWorksConnector : IDisposable
{
    private dynamic? _swApp;
    private bool _disposed;

    public dynamic? SwApp => _swApp;
    public bool IsConnected => _swApp != null;

    public void OpenNewInstance()
    {
        var swAppType = Type.GetTypeFromProgID("SldWorks.Application");
        if (swAppType == null)
            throw new InvalidOperationException("SolidWorks is not installed or COM registration is missing.");

        _swApp = Activator.CreateInstance(swAppType);
        if (_swApp == null)
            throw new InvalidOperationException("Failed to create SolidWorks instance.");

        _swApp.Visible = true;
    }

    public void Disconnect()
    {
        if (_swApp != null)
        {
            try { Marshal.ReleaseComObject(_swApp); }
            catch { /* ignore */ }
            _swApp = null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Disconnect();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
