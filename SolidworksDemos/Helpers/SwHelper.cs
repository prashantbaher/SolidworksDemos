using SldWorks;
using SwConst;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Helpers;

public class SwHelper : ISwHelper
{
    public SldWorks.SldWorks CreateSwInstance()
    {
        SldWorks.SldWorks swApp = new SldWorks.SldWorks();

        if (swApp == null)
            return null;

        swApp.Visible = true;
        return swApp;
    }

    public ModelDoc2 CreatePartDocument(SldWorks.SldWorks swApp)
    {
        string template = swApp.GetUserPreferenceStringValue(
            (int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(template))
            return null;

        ModelDoc2 swDoc = (ModelDoc2)swApp.NewDocument(template, 0, 0, 0);

        if (swDoc == null)
            return null;

        return swDoc;
    }

    public bool SelectPlaneAndInsertSketch(ModelDoc2 swDoc)
    {
        bool selected = swDoc.Extension.SelectByID2(
            "Right Plane", "PLANE", 0, 0, 0, false, 0, null,
            (int)swSelectOption_e.swSelectOptionDefault);

        if (!selected)
            return false;

        swDoc.SketchManager.InsertSketch(false);
        return true;
    }

    public double GetLengthConversionFactor(ModelDoc2 swDoc)
    {
        var lengthUnit = (swLengthUnit_e)swDoc.LengthUnit;

        switch (lengthUnit)
        {
            case swLengthUnit_e.swMM: return 0.001;
            case swLengthUnit_e.swCM: return 0.01;
            case swLengthUnit_e.swINCHES: return 0.0254;
            case swLengthUnit_e.swFEET: return 0.3048;
            default: return 0.001;
        }
    }

    public (double, double, double) ApplyUnitConversion(
        IPointViewModel inputPoint, double lengthConversionFactor)
    {
        double x = inputPoint.XPoint * lengthConversionFactor;
        double y = inputPoint.YPoint * lengthConversionFactor;
        double z = inputPoint.ZPoint * lengthConversionFactor;
        return (x, y, z);
    }

    public void CleanupAndExit(SldWorks.SldWorks swApp)
    {
        swApp.CloseAllDocuments(true);
        swApp.ExitApp();
    }
}
