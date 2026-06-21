using Spectre.Console;
using SldWorks;
using SwConst;
using SolidworksDemos.Constants;
using SolidworksDemos.Constants.Sketches;
using SolidworksDemos.Helpers;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;

namespace SolidworksDemos.Sketches;

public class EditSketchLine : ICrudAction
{
    private readonly ISwHelper _swHelper;

    public EditSketchLine(ISwHelper swHelper)
    {
        _swHelper = swHelper;
    }

    public void Execute()
    {
        string mode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(LineMessages.Prompts.EditMode)
                .AddChoices(LineMessages.Prompts.NewSwInstance, LineMessages.Prompts.RunningSwInstance));

        if (mode == LineMessages.Prompts.NewSwInstance)
        {
            double startX = AnsiConsole.Ask<double>(LineMessages.Prompts.StartPointX);
            double startY = AnsiConsole.Ask<double>(LineMessages.Prompts.StartPointY);
            double endX = AnsiConsole.Ask<double>(LineMessages.Prompts.EndPointX);
            double endY = AnsiConsole.Ask<double>(LineMessages.Prompts.EndPointY);

            double newStartX = AnsiConsole.Ask<double>(LineMessages.Prompts.NewStartPointX);
            double newStartY = AnsiConsole.Ask<double>(LineMessages.Prompts.NewStartPointY);
            double newEndX = AnsiConsole.Ask<double>(LineMessages.Prompts.NewEndPointX);
            double newEndY = AnsiConsole.Ask<double>(LineMessages.Prompts.NewEndPointY);

            var startPt = new PointViewModel { Header = LineMessages.Headers.StartPoint, XPoint = startX, YPoint = startY, ZPoint = 0 };
            var endPt = new PointViewModel { Header = LineMessages.Headers.EndPoint, XPoint = endX, YPoint = endY, ZPoint = 0 };
            var newStartPt = new PointViewModel { Header = LineMessages.Headers.NewStartPoint, XPoint = newStartX, YPoint = newStartY, ZPoint = 0 };
            var newEndPt = new PointViewModel { Header = LineMessages.Headers.NewEndPoint, XPoint = newEndX, YPoint = newEndY, ZPoint = 0 };

            string result = "";
            AnsiConsole.Status().Start(Menu.Messages.ProcessingMessage, ctx =>
            {
                result = RunEditSketchLineNewInstance(startPt, endPt, newStartPt, newEndPt);
            });
            AnsiConsole.MarkupLine(result);
        }
        else
        {
            double newStartX = AnsiConsole.Ask<double>(LineMessages.Prompts.NewStartPointX);
            double newStartY = AnsiConsole.Ask<double>(LineMessages.Prompts.NewStartPointY);
            double newEndX = AnsiConsole.Ask<double>(LineMessages.Prompts.NewEndPointX);
            double newEndY = AnsiConsole.Ask<double>(LineMessages.Prompts.NewEndPointY);

            var newStartPt = new PointViewModel { Header = LineMessages.Headers.NewStartPoint, XPoint = newStartX, YPoint = newStartY, ZPoint = 0 };
            var newEndPt = new PointViewModel { Header = LineMessages.Headers.NewEndPoint, XPoint = newEndX, YPoint = newEndY, ZPoint = 0 };

            string result = "";
            AnsiConsole.Status().Start(Menu.Messages.ProcessingMessage, ctx =>
            {
                result = RunEditSketchLineExistingInstance(newStartPt, newEndPt);
            });
            AnsiConsole.MarkupLine(result);
        }
    }

    internal string RunEditSketchLineNewInstance(
        IPointViewModel startPt, IPointViewModel endPt,
        IPointViewModel newStartPt, IPointViewModel newEndPt)
    {
        SldWorks.SldWorks swApp = _swHelper.CreateSwInstance();
        if (swApp == null)
            return LineMessages.Results.SwAppNotFound;

        ModelDoc2 swDoc = _swHelper.CreatePartDocument(swApp);
        if (swDoc == null)
        {
            _swHelper.CleanupAndExit(swApp);
            return LineMessages.Results.CreatePartFailed;
        }

        if (!_swHelper.SelectPlaneAndInsertSketch(swDoc))
        {
            _swHelper.CleanupAndExit(swApp);
            return LineMessages.Results.SelectPlaneFailed;
        }

        double factor = _swHelper.GetLengthConversionFactor(swDoc);
        var (x1, y1, z1) = _swHelper.ApplyUnitConversion(startPt, factor);
        var (x2, y2, z2) = _swHelper.ApplyUnitConversion(endPt, factor);

        SketchSegment segment = swDoc.SketchManager.CreateLine(x1, y1, z1, x2, y2, z2);
        if (segment == null)
        {
            _swHelper.CleanupAndExit(swApp);
            return LineMessages.Results.CreateLineFailed;
        }

        string result = EditLinePoints(swDoc, segment, newStartPt, newEndPt, factor);
        _swHelper.CleanupAndExit(swApp);
        return result;
    }

    internal string RunEditSketchLineExistingInstance(
        IPointViewModel newStartPt, IPointViewModel newEndPt)
    {
        SldWorks.SldWorks swApp = _swHelper.ConnectToRunningInstance();
        if (swApp == null)
            return LineMessages.Results.NoRunningSwInstance;

        ModelDoc2 swDoc = _swHelper.GetActiveDocument(swApp);
        if (swDoc == null)
            return LineMessages.Results.NoActiveDocument;

        if (swDoc.GetType() != (int)swDocumentTypes_e.swDocPART)
            return LineMessages.Results.NotPartDocument;

        Sketch activeSketch = swDoc.SketchManager.ActiveSketch;
        if (activeSketch == null)
            return LineMessages.Results.NoActiveSketch;

        object[] segments = (object[])activeSketch.GetSketchSegments();
        if (segments == null || segments.Length == 0)
            return LineMessages.Results.NoSketchSegments;

        SketchSegment lineSegment = null;
        foreach (object seg in segments)
        {
            var skSeg = (SketchSegment)seg;
            if (skSeg.GetType() == (int)swSketchSegments_e.swSketchLINE)
            {
                lineSegment = skSeg;
                break;
            }
        }

        if (lineSegment == null)
            return LineMessages.Results.NoLineFound;

        double factor = _swHelper.GetLengthConversionFactor(swDoc);
        return EditLinePoints(swDoc, lineSegment, newStartPt, newEndPt, factor);
    }

    private string EditLinePoints(ModelDoc2 swDoc, SketchSegment segment,
        IPointViewModel newStartPt, IPointViewModel newEndPt, double factor)
    {
        var skLine = (ISketchLine)segment;
        SketchPoint swStartPoint = skLine.IGetStartPoint2();
        SketchPoint swEndPoint = skLine.IGetEndPoint2();

        var (newX1, newY1, newZ1) = _swHelper.ApplyUnitConversion(newStartPt, factor);
        var (newX2, newY2, newZ2) = _swHelper.ApplyUnitConversion(newEndPt, factor);

        swStartPoint.SetCoords(newX1, newY1, newZ1);
        swEndPoint.SetCoords(newX2, newY2, newZ2);

        swDoc.EditRebuild3();
        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();

        return LineMessages.Results.LineEdited;
    }
}
