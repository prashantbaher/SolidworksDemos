using Spectre.Console;
using SldWorks;
using SolidworksDemos.Abstractions;
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

        SldWorks.SldWorks swApp = SwHelper.CreateSwInstance();
        if (swApp == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to find SolidWorks application.[/]");
            return;
        }

        ModelDoc2 swDoc = SwHelper.CreatePartDocument(swApp);
        if (swDoc == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to create new part document.[/]");
            return;
        }

        if (!SwHelper.SelectPlaneAndInsertSketch(swDoc))
        {
            SwHelper.CleanupAndExit(swApp);
            AnsiConsole.MarkupLine("[red]Failed to select Right Plane.[/]");
            return;
        }

        double factor = SwHelper.GetLengthConversionFactor(swDoc);
        var (x1, y1, z1) = SwHelper.ApplyUnitConversion(startPt, factor);
        var (x2, y2, z2) = SwHelper.ApplyUnitConversion(endPt, factor);

        SketchSegment segment = swDoc.SketchManager.CreateLine(x1, y1, z1, x2, y2, z2);
        if (segment == null)
        {
            SwHelper.CleanupAndExit(swApp);
            AnsiConsole.MarkupLine("[red]Failed to create sketch line.[/]");
            return;
        }

        swDoc.ClearSelection2(true);
        swDoc.ViewZoomtofit2();

        AnsiConsole.MarkupLine("[green]Sketch line successfully created.[/]");
    }
}
