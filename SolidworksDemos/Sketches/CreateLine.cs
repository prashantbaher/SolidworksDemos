using Spectre.Console;
using SldWorks;
using SolidworksDemos.Abstractions;
using SolidworksDemos.Constants;
using SolidworksDemos.Helpers;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;

namespace SolidworksDemos.Sketches;

public class CreateLine : ICrudAction
{
    public void Execute()
    {
        double startX = AnsiConsole.Ask<double>("Enter Start Point [green]X[/] (mm):");
        double startY = AnsiConsole.Ask<double>("Enter Start Point [green]Y[/] (mm):");
        double endX = AnsiConsole.Ask<double>("Enter End Point [green]X[/] (mm):");
        double endY = AnsiConsole.Ask<double>("Enter End Point [green]Y[/] (mm):");

        var startPt = new PointViewModel
        {
            Header = "Start Point",
            XPoint = startX,
            YPoint = startY,
            ZPoint = 0
        };

        var endPt = new PointViewModel
        {
            Header = "End Point",
            XPoint = endX,
            YPoint = endY,
            ZPoint = 0
        };

        string result = "";

        AnsiConsole.Status()
            .Start(Menu.ProcessingMessage, ctx =>
            {
                result = RunCreateLine(startPt, endPt);
            });

        AnsiConsole.MarkupLine(result);
    }

    private string RunCreateLine(IPointViewModel startPt, IPointViewModel endPt)
    {
        SldWorks.SldWorks swApp = SwHelper.CreateSwInstance();
        if (swApp == null)
            return "[red]Failed to find SolidWorks application.[/]";

        ModelDoc2 swDoc = SwHelper.CreatePartDocument(swApp);
        if (swDoc == null)
            return "[red]Failed to create new part document.[/]";

        if (!SwHelper.SelectPlaneAndInsertSketch(swDoc))
        {
            SwHelper.CleanupAndExit(swApp);
            return "[red]Failed to select Right Plane.[/]";
        }

        double factor = SwHelper.GetLengthConversionFactor(swDoc);
        var (x1, y1, z1) = SwHelper.ApplyUnitConversion(startPt, factor);
        var (x2, y2, z2) = SwHelper.ApplyUnitConversion(endPt, factor);

        SketchSegment segment = swDoc.SketchManager.CreateLine(x1, y1, z1, x2, y2, z2);
        if (segment == null)
        {
            SwHelper.CleanupAndExit(swApp);
            return "[red]Failed to create sketch line.[/]";
        }

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();

        return "[green]Sketch line successfully created.[/]";
    }
}
