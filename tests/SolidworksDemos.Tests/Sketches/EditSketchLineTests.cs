using System;
using Moq;
using SldWorks;
using SwConst;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;
using SolidworksDemos.Sketches;
using Xunit;

namespace SolidworksDemos.Tests.Sketches;

public class EditSketchLineTests
{
    private readonly IPointViewModel _startPt;
    private readonly IPointViewModel _endPt;
    private readonly IPointViewModel _newStartPt;
    private readonly IPointViewModel _newEndPt;

    public EditSketchLineTests()
    {
        _startPt = new PointViewModel { XPoint = 10, YPoint = 20, ZPoint = 0 };
        _endPt = new PointViewModel { XPoint = 100, YPoint = 200, ZPoint = 0 };
        _newStartPt = new PointViewModel { XPoint = 30, YPoint = 40, ZPoint = 0 };
        _newEndPt = new PointViewModel { XPoint = 150, YPoint = 250, ZPoint = 0 };
    }

    // ==================== Option A: New Instance ====================

    [Fact]
    public void RunEditSketchLineNewInstance_SwInstanceNull_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.CreateSwInstance()).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineNewInstance(_startPt, _endPt, _newStartPt, _newEndPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Never);
    }

    [Fact]
    public void RunEditSketchLineNewInstance_PartDocNull_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.CreateSwInstance()).Returns(new Mock<SldWorks.SldWorks>().Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineNewInstance(_startPt, _endPt, _newStartPt, _newEndPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Once);
    }

    [Fact]
    public void RunEditSketchLineNewInstance_PlaneSelectFails_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.CreateSwInstance()).Returns(new Mock<SldWorks.SldWorks>().Object);
        mockHelper.Setup(h => h.CreatePartDocument(It.IsAny<SldWorks.SldWorks>())).Returns(new Mock<ModelDoc2>().Object);
        mockHelper.Setup(h => h.SelectPlaneAndInsertSketch(It.IsAny<ModelDoc2>())).Returns(false);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineNewInstance(_startPt, _endPt, _newStartPt, _newEndPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Once);
    }

    [Fact]
    public void RunEditSketchLineNewInstance_CreateLineFails_ReturnsError()
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

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineNewInstance(_startPt, _endPt, _newStartPt, _newEndPt);

        Assert.Contains("Failed", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Once);
    }

    [Fact]
    public void RunEditSketchLineNewInstance_Success_ReturnsSuccess()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();
        var mockSegment = new Mock<SketchSegment>();
        var mockSkLine = mockSegment.As<ISketchLine>();
        var mockStartPt = new Mock<SketchPoint>();
        var mockStartPtItf = mockStartPt.As<ISketchPoint>();
        var mockEndPt = new Mock<SketchPoint>();
        var mockEndPtItf = mockEndPt.As<ISketchPoint>();

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

        mockSkLine.Setup(l => l.IGetStartPoint2()).Returns(mockStartPt.Object);
        mockSkLine.Setup(l => l.IGetEndPoint2()).Returns(mockEndPt.Object);
        mockStartPtItf.Setup(p => p.SetCoords(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
        mockEndPtItf.Setup(p => p.SetCoords(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineNewInstance(_startPt, _endPt, _newStartPt, _newEndPt);

        Assert.Contains("successfully", result);
        mockHelper.Verify(h => h.CleanupAndExit(It.IsAny<SldWorks.SldWorks>()), Times.Once);
    }

    // ==================== Option B: Running Instance ====================

    [Fact]
    public void RunEditSketchLineExistingInstance_NullInstance_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("No running", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_NullActiveDoc_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(new Mock<SldWorks.SldWorks>().Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("No active document", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_WrongDocType_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwDoc = new Mock<ModelDoc2>();

        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(new Mock<SldWorks.SldWorks>().Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockSwDoc.Setup(d => d.GetType()).Returns((int)swDocumentTypes_e.swDocDRAWING);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("not a part", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_NoActiveSketch_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();

        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockSwDoc.Setup(d => d.GetType()).Returns((int)swDocumentTypes_e.swDocPART);
        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSketchMgr.Setup(m => m.ActiveSketch).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("No active sketch", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_NoSegments_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();
        var mockSketch = new Mock<Sketch>();

        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockSwDoc.Setup(d => d.GetType()).Returns((int)swDocumentTypes_e.swDocPART);
        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSketchMgr.Setup(m => m.ActiveSketch).Returns(mockSketch.Object);
        mockSketch.Setup(s => s.GetSketchSegments()).Returns(() => null);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("No sketch segments", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_NoLineInSegments_ReturnsError()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();
        var mockSketch = new Mock<Sketch>();
        var mockArcSegment = new Mock<SketchSegment>();

        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockSwDoc.Setup(d => d.GetType()).Returns((int)swDocumentTypes_e.swDocPART);
        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSketchMgr.Setup(m => m.ActiveSketch).Returns(mockSketch.Object);
        mockSketch.Setup(s => s.GetSketchSegments()).Returns(new object[] { mockArcSegment.Object });
        mockArcSegment.Setup(s => s.GetType()).Returns((int)swSketchSegments_e.swSketchARC);

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("No line found", result);
    }

    [Fact]
    public void RunEditSketchLineExistingInstance_Success_ReturnsSuccess()
    {
        var mockHelper = new Mock<ISwHelper>();
        var mockSwApp = new Mock<SldWorks.SldWorks>();
        var mockSwDoc = new Mock<ModelDoc2>();
        var mockSketchMgr = new Mock<SketchManager>();
        var mockSketch = new Mock<Sketch>();
        var mockSegment = new Mock<SketchSegment>();
        var mockSkLine = mockSegment.As<ISketchLine>();
        var mockStartPt = new Mock<SketchPoint>();
        var mockStartPtItf = mockStartPt.As<ISketchPoint>();
        var mockEndPt = new Mock<SketchPoint>();
        var mockEndPtItf = mockEndPt.As<ISketchPoint>();

        mockHelper.Setup(h => h.ConnectToRunningInstance()).Returns(mockSwApp.Object);
        mockHelper.Setup(h => h.GetActiveDocument(It.IsAny<SldWorks.SldWorks>())).Returns(mockSwDoc.Object);
        mockHelper.Setup(h => h.GetLengthConversionFactor(It.IsAny<ModelDoc2>())).Returns(0.001);
        mockHelper.Setup(h => h.ApplyUnitConversion(It.IsAny<IPointViewModel>(), 0.001))
            .Returns((IPointViewModel p, double f) => (p.XPoint * f, p.YPoint * f, p.ZPoint * f));

        mockSwDoc.Setup(d => d.GetType()).Returns((int)swDocumentTypes_e.swDocPART);
        mockSwDoc.Setup(d => d.SketchManager).Returns(mockSketchMgr.Object);
        mockSwDoc.Setup(d => d.ClearSelection2(It.IsAny<bool>()));
        mockSwDoc.Setup(d => d.ViewZoomtofit2());
        mockSwDoc.Setup(d => d.EditRebuild3());
        mockSketchMgr.Setup(m => m.ActiveSketch).Returns(mockSketch.Object);
        mockSketch.Setup(s => s.GetSketchSegments()).Returns(new object[] { mockSegment.Object });
        mockSegment.Setup(s => s.GetType()).Returns((int)swSketchSegments_e.swSketchLINE);

        mockSkLine.Setup(l => l.IGetStartPoint2()).Returns(mockStartPt.Object);
        mockSkLine.Setup(l => l.IGetEndPoint2()).Returns(mockEndPt.Object);
        mockStartPtItf.Setup(p => p.SetCoords(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
        mockEndPtItf.Setup(p => p.SetCoords(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));

        var action = new EditSketchLine(mockHelper.Object);

        var result = action.RunEditSketchLineExistingInstance(_newStartPt, _newEndPt);

        Assert.Contains("successfully", result);
    }
}
