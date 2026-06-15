using SldWorks;
using SwConst;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Services;

public class SketchService : ISketchService
{
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

    public bool SelectPlaneAndInsertSketch(ModelDoc2 swDoc, out string message)
    {
        var selected = swDoc.Extension.SelectByID2(
            "Right Plane", "PLANE", 0, 0, 0, false, 0, null,
            (int)swSelectOption_e.swSelectOptionDefault);

        if (!selected)
        {
            message = Constants.CreateLineErrors.PlaneSelectFailed;
            return false;
        }

        swDoc.SketchManager.InsertSketch(false);
        message = string.Empty;
        return true;
    }
}
