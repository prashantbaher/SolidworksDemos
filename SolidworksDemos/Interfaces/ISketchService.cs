using SldWorks;

namespace SolidworksDemos.Interfaces;

public interface ISketchService
{
    bool GetActiveSketch(ModelDoc2 swDoc, out ISketch sketch, out string message);
    bool SelectPlaneAndInsertSketch(ModelDoc2 swDoc, out string message);
}
