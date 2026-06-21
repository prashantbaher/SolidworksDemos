using Spectre.Console;
using SldWorks;
using SolidworksDemos.Constants;
using SolidworksDemos.Constants.Sketches;
using SolidworksDemos.Helpers;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;

namespace SolidworksDemos.Sketches;

public class CreateLine : ICrudAction
{
    private readonly ISwHelper _swHelper;

    public CreateLine(ISwHelper swHelper)
    {
        _swHelper = swHelper;
    }

    public void Execute()
    {
        double startX = AnsiConsole.Ask<double>(LineMessages.Prompts.StartPointX);
        double startY = AnsiConsole.Ask<double>(LineMessages.Prompts.StartPointY);
        double endX = AnsiConsole.Ask<double>(LineMessages.Prompts.EndPointX);
        double endY = AnsiConsole.Ask<double>(LineMessages.Prompts.EndPointY);

        var startPt = new PointViewModel
        {
            Header = LineMessages.Headers.StartPoint,
            XPoint = startX,
            YPoint = startY,
            ZPoint = 0
        };

        var endPt = new PointViewModel
        {
            Header = LineMessages.Headers.EndPoint,
            XPoint = endX,
            YPoint = endY,
            ZPoint = 0
        };

        string result = "";

        AnsiConsole.Status()
            .Start(Menu.Messages.ProcessingMessage, ctx =>
            {
                result = RunCreateLine(startPt, endPt);
            });

        AnsiConsole.MarkupLine(result);
    }

    internal string RunCreateLine(IPointViewModel startPt, IPointViewModel endPt)
    {
        SldWorks.SldWorks swApp = _swHelper.CreateSwInstance();
        if (swApp == null)
            return LineMessages.Results.SwAppNotFound;

        ModelDoc2 swDoc = _swHelper.CreatePartDocument(swApp);
        if (swDoc == null)
            return LineMessages.Results.CreatePartFailed;

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

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();

        return LineMessages.Results.LineCreated;
    }
}
