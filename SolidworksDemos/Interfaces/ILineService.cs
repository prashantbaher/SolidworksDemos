using SldWorks;

namespace SolidworksDemos.Interfaces;

public interface ILineService
{
    bool FindFirstLine(ISketch sketch, out ISketchLine line,
        out SketchPoint startPoint, out SketchPoint endPoint, out string message);
    bool UpdatePoint(SketchPoint point, double x, double y, out string message);
    bool CreateLine(SketchManager sketchManager,
        double x1, double y1, double z1,
        double x2, double y2, double z2, out string message);
}
