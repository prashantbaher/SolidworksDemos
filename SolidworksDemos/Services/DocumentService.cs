using SldWorks;
using SwConst;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Services;

public class DocumentService : IDocumentService
{
    public bool GetActiveDocument(SldWorks.SldWorks swApp, out ModelDoc2 swDoc, out string message)
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

    public bool CreatePartDocument(out SldWorks.SldWorks swApp, out ModelDoc2 swDoc, out string message)
    {
        swApp = new SldWorks.SldWorks();

        if (swApp == null)
        {
            message = Constants.Errors.SolidworksFailed;
            swDoc = null!;
            return false;
        }

        swApp.Visible = true;

        var template = swApp.GetUserPreferenceStringValue(
            (int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

        if (string.IsNullOrEmpty(template))
        {
            message = Constants.CreateLineErrors.TemplateEmpty;
            swDoc = null!;
            return false;
        }

        swDoc = swApp.NewDocument(template, 0, 0, 0);

        if (swDoc == null)
        {
            message = Constants.CreateLineErrors.DocFailed;
            return false;
        }

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

    public double GetLengthConversionFactor(ModelDoc2 swDoc)
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
}
