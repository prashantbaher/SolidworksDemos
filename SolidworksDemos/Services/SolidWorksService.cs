using SolidWorks.Interop.sldworks;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Services;

public class SolidWorksService : ISolidWorksService
{
    public bool CreateSolidworksInstance(out SldWorks swApp, out string message)
    {
        swApp = new SldWorks();

        if (swApp == null)
        {
            message = Constants.Errors.SolidworksFailed;
            return false;
        }

        swApp.Visible = true;
        message = string.Empty;
        return true;
    }

    public bool GetActiveDocument(SldWorks swApp, out ModelDoc2 swDoc, out string message)
    {
        swDoc = (ModelDoc2)swApp.ActiveDoc;

        if (swDoc == null)
        {
            message = Constants.Errors.NoActiveDoc;
            return false;
        }

        message = string.Empty;
        return true;
    }

    public bool GetActiveSketch(ModelDoc2 swDoc, out ISketch sketch, out string message)
    {
        sketch = (ISketch)swDoc.SketchManager.ActiveSketch;

        if (sketch == null)
        {
            message = Constants.Errors.NoActiveSketch;
            return false;
        }

        message = string.Empty;
        return true;
    }

    public bool FindFirstLine(ISketch sketch, out ISketchLine line,
        out SketchPoint startPoint, out SketchPoint endPoint, out string message)
    {
        line = null!;
        startPoint = null!;
        endPoint = null!;

        var segments = (object[])sketch.GetSketchSegments();

        if (segments == null || segments.Length == 0)
        {
            message = Constants.Errors.NoSegmentsFound;
            return false;
        }

        foreach (var segment in segments)
        {
            if (segment is ISketchLine sketchLine)
            {
                line = sketchLine;
                startPoint = line.IGetStartPoint2();
                endPoint = line.IGetEndPoint2();

                message = string.Empty;
                return true;
            }
        }

        message = Constants.Errors.NoLineFound;
        return false;
    }

    public bool UpdateSketchPoint(SketchPoint point, double x, double y, out string message)
    {
        if (point == null)
        {
            message = Constants.Errors.UpdateFailed;
            return false;
        }

        var z = (double)point.Z;
        point.SetCoords(x / 1000.0, y / 1000.0, z / 1000.0);

        message = string.Empty;
        return true;
    }

    public bool RebuildAndZoom(ModelDoc2 swDoc, out string message)
    {
        var result = swDoc.EditRebuild3();

        if (!result)
        {
            message = Constants.Errors.RebuildFailed;
            return false;
        }

        swDoc.ViewZoomtofit2();
        message = string.Empty;
        return true;
    }
}
