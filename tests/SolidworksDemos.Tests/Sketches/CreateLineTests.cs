using System;
using Moq;
using SldWorks;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;
using SolidworksDemos.Sketches;
using Xunit;

namespace SolidworksDemos.Tests.Sketches;

public class CreateLineTests
{
    private readonly IPointViewModel _startPt;
    private readonly IPointViewModel _endPt;

    public CreateLineTests()
    {
        _startPt = new PointViewModel { XPoint = 10, YPoint = 20, ZPoint = 0 };
        _endPt = new PointViewModel { XPoint = 100, YPoint = 200, ZPoint = 0 };
    }

    [Fact]
    public void RunCreateLine_SwInstanceNull_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.CreateSwInstance()).Returns(() => null);

        var action = new CreateLine(mockHelper.Object);

        var result = action.RunCreateLine(_startPt, _endPt);

        Assert.Contains("Failed", result);
    }

    [Fact]
    public void RunCreateLine_PartDocNull_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.CreateSwInstance()).Returns(new Mock<SldWorks.SldWorks>().Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(() => null);

        var action = new CreateLine(mockHelper.Object);

        var result = action.RunCreateLine(_startPt, _endPt);

        Assert.Contains("Failed", result);
    }

    [Fact]
    public void RunCreateLine_PlaneSelectFails_CallsCleanupAndExit_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();

        mockHelper.Setup(h => h.CreateSwInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockHelper.Setup(h => h.SelectPlaneAndInsertSketch(It.IsAny<ModelDoc2>())).Returns(false);

        var action = new CreateLine(mockHelper.Object);

        var result = action.RunCreateLine(_startPt, _endPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(mockSwApp.Object), Times.Once);
    }

    [Fact]
    public void RunCreateLine_CreateLineFails_CallsCleanupAndExit_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();

        mockHelper.Setup(h => h.CreateSwInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockHelper.Setup(h => h.SelectPlaneAndInsertSketch(It.IsAny<ModelDoc2>())).Returns(true);
        mockHelper.Setup(h => h.GetLengthConversionFactor(It.IsAny<ModelDoc2>())).Returns(0.001);
        mockHelper.Setup(h => h.ApplyUnitConversion(It.IsAny<IPointViewModel>(), 0.001))
            .Returns((IPointViewModel p, double f) => (p.XPoint * f, p.YPoint * f, p.ZPoint * f));

        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSketchMgr.Setup(m => m.CreateLine(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(),
            It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(() => null);

        var action = new CreateLine(mockHelper.Object);

        var result = action.RunCreateLine(_startPt, _endPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(mockSwApp.Object), Times.Once);
    }

    [Fact]
    public void RunCreateLine_AllSucceed_ReturnsSuccess()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();
        var mockSegment = new Mock<SketchSegment>();

        mockHelper.Setup(h => h.CreateSwInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockHelper.Setup(h => h.SelectPlaneAndInsertSketch(It.IsAny<ModelDoc2>())).Returns(true);
        mockHelper.Setup(h => h.GetLengthConversionFactor(It.IsAny<ModelDoc2>())).Returns(0.001);
        mockHelper.Setup(h => h.ApplyUnitConversion(It.IsAny<IPointViewModel>(), 0.001))
            .Returns((IPointViewModel p, double f) => (p.XPoint * f, p.YPoint * f, p.ZPoint * f));

        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSwDoc.Setup(d => d.ClearSelection2(It.IsAny<bool>()));
        mockSwDoc.Setup(d => d.ViewZoomtofit2());
        mockSketchMgr.Setup(m => m.CreateLine(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(),
            It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(mockSegment.Object);

        var action = new CreateLine(mockHelper.Object);

        var result = action.RunCreateLine(_startPt, _endPt);

        Assert.Contains("successfully", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Never);
    }
}
