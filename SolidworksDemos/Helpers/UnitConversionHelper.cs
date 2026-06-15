using SwConst;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Helpers;

public class UnitConversionHelper : IUnitConversionHelper
{
    public double AngleConversionFactor { get; set; }
    public double LengthConversionFactor { get; set; }

    public void UnitConversion(swLengthUnit_e swUnit)
    {
        switch (swUnit)
        {
            case swLengthUnit_e.swMM:
                LengthConversionFactor = 0.001;
                break;
            case swLengthUnit_e.swCM:
                LengthConversionFactor = 0.01;
                break;
            case swLengthUnit_e.swINCHES:
                LengthConversionFactor = 0.0254;
                break;
            case swLengthUnit_e.swFEET:
                LengthConversionFactor = 0.3048;
                break;
            default:
                LengthConversionFactor = 0.001;
                break;
        }
    }
}
