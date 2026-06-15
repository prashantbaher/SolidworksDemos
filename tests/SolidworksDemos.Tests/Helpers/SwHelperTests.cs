using System;
using Moq;
using SldWorks;
using SwConst;
using SolidworksDemos.Helpers;
using SolidworksDemos.Interfaces;
using Xunit;

namespace SolidworksDemos.Tests.Helpers;

public class SwHelperTests
{
    private readonly SwHelper _helper = new SwHelper();

    [Theory]
    [InlineData(swLengthUnit_e.swMM, 0.001)]
    [InlineData(swLengthUnit_e.swCM, 0.01)]
    [InlineData(swLengthUnit_e.swINCHES, 0.0254)]
    [InlineData(swLengthUnit_e.swFEET, 0.3048)]
    public void GetLengthConversionFactor_Returns_CorrectFactor(swLengthUnit_e unit, double expected)
    {
        var mockDoc = new Mock<ModelDoc2>();
        mockDoc.Setup(d => d.LengthUnit).Returns((int)unit);

        var result = _helper.GetLengthConversionFactor(mockDoc.Object);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ApplyUnitConversion_MultipliesCoordinatesByFactor()
    {
        var pt = new Mock<IPointViewModel>();
        pt.Setup(p => p.XPoint).Returns(10);
        pt.Setup(p => p.YPoint).Returns(20);
        pt.Setup(p => p.ZPoint).Returns(30);

        var (x, y, z) = _helper.ApplyUnitConversion(pt.Object, 2.0);

        Assert.Equal(20, x);
        Assert.Equal(40, y);
        Assert.Equal(60, z);
    }

    [Fact]
    public void ApplyUnitConversion_ZeroFactor_ReturnsZero()
    {
        var pt = new Mock<IPointViewModel>();
        pt.Setup(p => p.XPoint).Returns(10);
        pt.Setup(p => p.YPoint).Returns(20);
        pt.Setup(p => p.ZPoint).Returns(30);

        var (x, y, z) = _helper.ApplyUnitConversion(pt.Object, 0);

        Assert.Equal(0, x);
        Assert.Equal(0, y);
        Assert.Equal(0, z);
    }
}
