using SldWorks;

namespace SolidworksDemos.Interfaces;

public interface IDocumentService
{
    bool GetActiveDocument(SldWorks.SldWorks swApp, out ModelDoc2 swDoc, out string message);
    bool CreatePartDocument(out SldWorks.SldWorks swApp, out ModelDoc2 swDoc, out string message);
    bool RebuildAndZoom(ModelDoc2 swDoc, out string message);
    double GetLengthConversionFactor(ModelDoc2 swDoc);
}
