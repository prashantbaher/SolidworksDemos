using System.Runtime.InteropServices;
using Microsoft.Win32;
using SldWorks;
using SwConst;
using SwApp = SldWorks.SldWorks;

namespace SolidworksDemos;

public static class SolidWorksHelper
{
    public static SwApp? ConnectToSolidWorks()
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

        SwApp swApp;
        try
        {
            swApp = (SwApp)Marshal.GetActiveObject(latestProgId);
        }
        catch
        {
            swApp = new SwApp();
        }

        if (swApp == null)
            return null;

        swApp.Visible = true;
        return swApp;
    }

    public static ModelDoc2? GetActiveDocument(SwApp swApp)
    {
        return (ModelDoc2?)swApp.ActiveDoc;
    }

    public static ISketch? GetActiveSketch(ModelDoc2 swDoc)
    {
        return (ISketch?)swDoc.SketchManager.ActiveSketch;
    }

    public static object[]? GetSketchSegments(ISketch sketch)
    {
        return (object[]?)sketch.GetSketchSegments();
    }

    public static ISketchLine? FindFirstSketchLine(object[] segments)
    {
        foreach (var segment in segments)
        {
            if (segment is ISketchLine line)
                return line;
        }
        return null;
    }

    public static SketchPoint GetStartPoint(ISketchLine line)
    {
        return line.IGetStartPoint2();
    }

    public static SketchPoint GetEndPoint(ISketchLine line)
    {
        return line.IGetEndPoint2();
    }

    public static bool UpdatePoint(SketchPoint point, double x, double y)
    {
        if (point == null)
            return false;

        var z = (double)point.Z;
        point.SetCoords(x / 1000.0, y / 1000.0, z / 1000.0);
        return true;
    }

    public static bool RebuildAndZoom(ModelDoc2 swDoc)
    {
        if (!swDoc.EditRebuild3())
            return false;

        swDoc.ViewZoomtofit2();
        return true;
    }

    public static SwApp? CreateNewSwInstance()
    {
        var swApp = new SwApp();
        if (swApp == null)
            return null;

        swApp.Visible = true;
        return swApp;
    }

    public static ModelDoc2? CreateNewDocument(SwApp swApp)
    {
        var template = swApp.GetUserPreferenceStringValue(
            (int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(template))
            return null;

        return (ModelDoc2?)swApp.NewDocument(template, 0, 0, 0);
    }

    public static bool SelectPlaneAndInsertSketch(ModelDoc2 swDoc)
    {
        var selected = swDoc.Extension.SelectByID2(
            "Right Plane", "PLANE", 0, 0, 0, false, 0, null,
            (int)swSelectOption_e.swSelectOptionDefault);

        if (!selected)
            return false;

        swDoc.SketchManager.InsertSketch(false);
        return true;
    }

    public static double GetLengthConversionFactor(ModelDoc2 swDoc)
    {
        var lengthUnit = (swLengthUnit_e)swDoc.LengthUnit;

        return lengthUnit switch
        {
            swLengthUnit_e.swMM => 0.001,
            swLengthUnit_e.swCM => 0.01,
            swLengthUnit_e.swINCHES => 0.0254,
            swLengthUnit_e.swFEET => 0.3048,
            _ => 0.001
        };
    }

    public static bool CreateLine(SketchManager sketchManager,
        double x1, double y1, double z1,
        double x2, double y2, double z2)
    {
        return sketchManager.CreateLine(x1, y1, z1, x2, y2, z2) != null;
    }
}
