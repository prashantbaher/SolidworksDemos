using SldWorks;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Services;

public class LineService : ILineService
{
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

    public bool UpdatePoint(SketchPoint point, double x, double y, out string message)
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

    public bool CreateLine(SketchManager sketchManager,
        double x1, double y1, double z1,
        double x2, double y2, double z2, out string message)
    {
        var sketchSegment = sketchManager.CreateLine(x1, y1, z1, x2, y2, z2);

        if (sketchSegment == null)
        {
            message = Constants.CreateLineErrors.LineFailed;
            return false;
        }

        message = string.Empty;
        return true;
    }
}
