using Spectre.Console;
using SldWorks;
using SwConst;
using SolidworksDemos.Abstractions;
using SolidworksDemos.Constants;
using SolidworksDemos.Helpers;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;

namespace SolidworksDemos.Sketches;

public class EditSketchLine : ICrudAction
{
    private readonly ISwHelper _swHelper;

    public EditSketchLine() : this(new SwHelper())
    {
    }

    public EditSketchLine(ISwHelper swHelper)
    {
        _swHelper = swHelper;
    }

    public void Execute()
    {
        string mode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select [green]edit mode[/]")
                .AddChoices("New SW Instance", "Running SW Instance"));

        if (mode == "New SW Instance")
        {
            double startX = AnsiConsole.Ask<double>("Enter Start Point [green]X[/] (mm):");
            double startY = AnsiConsole.Ask<double>("Enter Start Point [green]Y[/] (mm):");
            double endX = AnsiConsole.Ask<double>("Enter End Point [green]X[/] (mm):");
            double endY = AnsiConsole.Ask<double>("Enter End Point [green]Y[/] (mm):");

            double newStartX = AnsiConsole.Ask<double>("Enter New Start Point [green]X[/] (mm):");
            double newStartY = AnsiConsole.Ask<double>("Enter New Start Point [green]Y[/] (mm):");
            double newEndX = AnsiConsole.Ask<double>("Enter New End Point [green]X[/] (mm):");
            double newEndY = AnsiConsole.Ask<double>("Enter New End Point [green]Y[/] (mm):");

            var startPt = new PointViewModel { Header = "Start Point", XPoint = startX, YPoint = startY, ZPoint = 0 };
            var endPt = new PointViewModel { Header = "End Point", XPoint = endX, YPoint = endY, ZPoint = 0 };
            var newStartPt = new PointViewModel { Header = "New Start Point", XPoint = newStartX, YPoint = newStartY, ZPoint = 0 };
            var newEndPt = new PointViewModel { Header = "New End Point", XPoint = newEndX, YPoint = newEndY, ZPoint = 0 };

            string result = "";
            AnsiConsole.Status().Start(Menu.ProcessingMessage, ctx =>
            {
                result = RunEditSketchLineNewInstance(startPt, endPt, newStartPt, newEndPt);
            });
            AnsiConsole.MarkupLine(result);
        }
        else
        {
            double newStartX = AnsiConsole.Ask<double>("Enter New Start Point [green]X[/] (mm):");
            double newStartY = AnsiConsole.Ask<double>("Enter New Start Point [green]Y[/] (mm):");
            double newEndX = AnsiConsole.Ask<double>("Enter New End Point [green]X[/] (mm):");
            double newEndY = AnsiConsole.Ask<double>("Enter New End Point [green]Y[/] (mm):");

            var newStartPt = new PointViewModel { Header = "New Start Point", XPoint = newStartX, YPoint = newStartY, ZPoint = 0 };
            var newEndPt = new PointViewModel { Header = "New End Point", XPoint = newEndX, YPoint = newEndY, ZPoint = 0 };

            string result = "";
            AnsiConsole.Status().Start(Menu.ProcessingMessage, ctx =>
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
            return "[red]Failed to find SolidWorks application.[/]";

        ModelDoc2 swDoc = _swHelper.CreatePartDocument(swApp);
        if (swDoc == null)
        {
            _swHelper.CleanupAndExit(swApp);
            return "[red]Failed to create new part document.[/]";
        }

        if (!_swHelper.SelectPlaneAndInsertSketch(swDoc))
        {
            _swHelper.CleanupAndExit(swApp);
            return "[red]Failed to select Right Plane.[/]";
        }

        double factor = _swHelper.GetLengthConversionFactor(swDoc);
        var (x1, y1, z1) = _swHelper.ApplyUnitConversion(startPt, factor);
        var (x2, y2, z2) = _swHelper.ApplyUnitConversion(endPt, factor);

        SketchSegment segment = swDoc.SketchManager.CreateLine(x1, y1, z1, x2, y2, z2);
        if (segment == null)
        {
            _swHelper.CleanupAndExit(swApp);
            return "[red]Failed to create sketch line.[/]";
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
            return "[red]No running SolidWorks instance found.[/]";

        ModelDoc2 swDoc = _swHelper.GetActiveDocument(swApp);
        if (swDoc == null)
            return "[red]No active document open.[/]";

        if (swDoc.GetType() != (int)swDocumentTypes_e.swDocPART)
            return "[red]Active document is not a part document.[/]";

        Sketch activeSketch = swDoc.SketchManager.ActiveSketch;
        if (activeSketch == null)
            return "[red]No active sketch found. Enter sketch mode first.[/]";

        object[] segments = (object[])activeSketch.GetSketchSegments();
        if (segments == null || segments.Length == 0)
            return "[red]No sketch segments found.[/]";

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
            return "[red]No line found in the active sketch.[/]";

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

        return "[green]Sketch line successfully edited.[/]";
    }
}
