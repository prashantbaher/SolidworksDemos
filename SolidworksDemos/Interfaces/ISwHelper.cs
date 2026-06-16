using SldWorks;
using SwConst;

namespace SolidworksDemos.Interfaces;

public interface ISwHelper
{
    SldWorks.SldWorks CreateSwInstance();
    SldWorks.SldWorks ConnectToRunningInstance();
    ModelDoc2 CreatePartDocument(SldWorks.SldWorks swApp);
    ModelDoc2 GetActiveDocument(SldWorks.SldWorks swApp);
    bool SelectPlaneAndInsertSketch(ModelDoc2 swDoc);
    double GetLengthConversionFactor(ModelDoc2 swDoc);
    (double, double, double) ApplyUnitConversion(IPointViewModel inputPoint, double lengthConversionFactor);
    void CleanupAndExit(SldWorks.SldWorks swApp);
}
