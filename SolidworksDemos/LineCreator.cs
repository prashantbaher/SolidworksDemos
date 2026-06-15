using SldWorks;
using SwConst;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos;

public class LineCreator
{
    public string messageToShow;
    public IPointViewModel StartPointViewModel;
    public IPointViewModel EndPointViewModel;
    public IUnitConversionHelper conversionHelper;

    public bool CreateLineMethod()
    {
        ModelDoc2 swDoc;
        if (CreateSolidworksInstance(out swDoc) == false)
            return false;

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();
        messageToShow = "Sketch line successfully created.";
        return true;
    }

    public virtual bool CreateSolidworksInstance(out ModelDoc2 swDoc)
    {
        SldWorks.SldWorks swApp = new SldWorks.SldWorks();

        if (swApp == null)
        {
            messageToShow = "Failed to find Solidworks application.";
            swDoc = null;
            return false;
        }

        swApp.Visible = true;

        return CreatePartDocument(swApp, out swDoc);
    }

    public bool CreatePartDocument(SldWorks.SldWorks swApp, out ModelDoc2 swDoc)
    {
        string defaultTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(defaultTemplate))
        {
            messageToShow = "Default part template is empty.";
            swDoc = null;
            return false;
        }

        swDoc = swApp.NewDocument(defaultTemplate, 0, 0, 0);

        if (swDoc == null)
        {
            messageToShow = "Failed to create new Part document.";
            return false;
        }

        return SelectSketchPlane(swApp, swDoc);
    }

    public bool SelectSketchPlane(SldWorks.SldWorks swApp, ModelDoc2 swDoc)
    {
        bool boolStatus = swDoc.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

        if (boolStatus == false)
        {
            messageToShow = "Failed to select Right Plane";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        swDoc.SketchManager.InsertSketch(false);

        double x1, y1, z1, x2, y2, z2;
        var lengthUnit = swDoc.LengthUnit;

        conversionHelper.UnitConversion((swLengthUnit_e)lengthUnit);

        (x1, y1, z1) = ApplyUnitConversion(StartPointViewModel, conversionHelper.LengthConversionFactor);
        (x2, y2, z2) = ApplyUnitConversion(EndPointViewModel, conversionHelper.LengthConversionFactor);
        SketchSegment sketchSegment = null;
        return CreateLine(sketchSegment, x1, y1, z1, swApp, swDoc.SketchManager, x2, y2, z2);
    }

    public (double, double, double) ApplyUnitConversion(IPointViewModel inputPoint, double lengthConversionFactor)
    {
        double X, Y, Z;

        X = inputPoint.XPoint * lengthConversionFactor;
        Y = inputPoint.YPoint * lengthConversionFactor;
        Z = inputPoint.ZPoint * lengthConversionFactor;

        return (X, Y, Z);
    }

    public bool CreateLine(SketchSegment sketchSegment, double x1, double y1, double z1, SldWorks.SldWorks swApp, SketchManager swSketchManager, double x2, double y2, double z2)
    {
        sketchSegment = swSketchManager.CreateLine(x1, y1, z1, x2, y2, z2);

        if (sketchSegment == null)
        {
            messageToShow = "Failed to Create Sketch line.";
            swApp.CloseAllDocuments(true);
            swApp.ExitApp();
            return false;
        }

        return true;
    }
}
