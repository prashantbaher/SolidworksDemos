using System.Runtime.InteropServices;
using Microsoft.Win32;
using SldWorks;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Services;

public class ApplicationService : IApplicationService
{
    private static string GetLatestSolidWorksProgId()
    {
        var regex = new System.Text.RegularExpressions.Regex(@"^SldWorks\.Application\.(\d+)$");
        var maxVersion = 0;
        var latestProgId = "SldWorks.Application";

        foreach (var keyName in Registry.ClassesRoot.GetSubKeyNames())
        {
            var match = regex.Match(keyName);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var v) && v > maxVersion)
            {
                maxVersion = v;
                latestProgId = keyName;
            }
        }

        return latestProgId;
    }

    public bool CreateInstance(out SldWorks.SldWorks swApp, out string message)
    {
        try
        {
            var progId = GetLatestSolidWorksProgId();
            swApp = (SldWorks.SldWorks)Marshal.GetActiveObject(progId);
        }
        catch
        {
            swApp = new SldWorks.SldWorks();
        }

        if (swApp == null)
        {
            message = Constants.Errors.SolidworksFailed;
            return false;
        }

        swApp.Visible = true;
        message = string.Empty;
        return true;
    }
}
