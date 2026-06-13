using SldWorks;

namespace SolidworksDemos.Interfaces;

public interface ISolidWorksService
{
    bool CreateSolidworksInstance(out SldWorks.SldWorks swApp, out string message);
    bool GetActiveDocument(SldWorks.SldWorks swApp, out ModelDoc2 swDoc, out string message);
    bool GetActiveSketch(ModelDoc2 swDoc, out ISketch sketch, out string message);
    bool FindFirstLine(ISketch sketch, out ISketchLine line,
        out SketchPoint startPoint, out SketchPoint endPoint, out string message);
    bool UpdateSketchPoint(SketchPoint point, double x, double y, out string message);
    bool RebuildAndZoom(ModelDoc2 swDoc, out string message);
}
