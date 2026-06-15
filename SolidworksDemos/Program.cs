using Spectre.Console;
using SolidworksDemos;

AnsiConsole.Write(new Rule("[yellow]SolidworksDemos[/]").RuleStyle("yellow"));

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title(Constants.Menu.Title)
            .AddChoices(Constants.Menu.HelloOption, Constants.Menu.EditLineOption,
                        Constants.Menu.CreateLineOption, Constants.Menu.ExitOption));

    if (choice == Constants.Menu.ExitOption)
    {
        AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
        AnsiConsole.MarkupLine("[dim]Press any key to close...[/]");
        Console.ReadKey();
        return;
    }

    if (choice == Constants.Menu.HelloOption)
    {
        SayHello();
    }
    else if (choice == Constants.Menu.EditLineOption)
    {
        EditSketchLine();
    }
    else if (choice == Constants.Menu.CreateLineOption)
    {
        CreateSketchLine();
    }

    AnsiConsole.WriteLine();
}

static void SayHello()
{
    var name = AnsiConsole.Ask<string>(Constants.HelloMessages.NamePrompt);
    AnsiConsole.MarkupLine(string.Format(Constants.HelloMessages.GreetingTemplate, name));
}

static void EditSketchLine()
{
    var swApp = SolidWorksHelper.ConnectToSolidWorks();
    if (swApp == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.SolidworksFailed));
        return;
    }

    var swDoc = SolidWorksHelper.GetActiveDocument(swApp);
    if (swDoc == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.NoActiveDoc));
        return;
    }

    var sketch = SolidWorksHelper.GetActiveSketch(swDoc);
    if (sketch == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.NoActiveSketch));
        return;
    }

    var segments = SolidWorksHelper.GetSketchSegments(sketch);
    if (segments == null || segments.Length == 0)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.NoSegmentsFound));
        return;
    }

    var line = SolidWorksHelper.FindFirstSketchLine(segments);
    if (line == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.NoLineFound));
        return;
    }

    var startPt = SolidWorksHelper.GetStartPoint(line);
    var endPt = SolidWorksHelper.GetEndPoint(line);

    AnsiConsole.MarkupLine(string.Format(Constants.EditLineMessages.PointInfoFormat,
        (double)startPt.X, (double)startPt.Y, (double)endPt.X, (double)endPt.Y));

    var pointChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title(Constants.EditLineMessages.PointSelectionTitle)
            .AddChoices(Constants.EditLineMessages.StartPoint, Constants.EditLineMessages.EndPoint));

    var newX = AnsiConsole.Ask<double>(Constants.EditLineMessages.PromptX);
    var newY = AnsiConsole.Ask<double>(Constants.EditLineMessages.PromptY);

    var chosenPt = pointChoice == Constants.EditLineMessages.StartPoint ? startPt : endPt;
    var pointName = pointChoice == Constants.EditLineMessages.StartPoint ? "Start" : "End";
    var oldX = pointChoice == Constants.EditLineMessages.StartPoint ? (double)startPt.X : (double)endPt.X;
    var oldY = pointChoice == Constants.EditLineMessages.StartPoint ? (double)startPt.Y : (double)endPt.Y;

    if (!SolidWorksHelper.UpdatePoint(chosenPt, newX, newY))
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.UpdateFailed));
        return;
    }

    if (!SolidWorksHelper.RebuildAndZoom(swDoc))
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.RebuildFailed));
        return;
    }

    AnsiConsole.MarkupLine(string.Format(Constants.EditLineMessages.SuccessFormat,
        pointName, oldX, oldY, newX, newY));
}

static void CreateSketchLine()
{
    var startX = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptStartX);
    var startY = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptStartY);
    var endX = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptEndX);
    var endY = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptEndY);

    var swApp = SolidWorksHelper.CreateNewSwInstance();
    if (swApp == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.Errors.SolidworksFailed));
        return;
    }

    var swDoc = SolidWorksHelper.CreateNewDocument(swApp);
    if (swDoc == null)
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.CreateLineErrors.DocFailed));
        return;
    }

    if (!SolidWorksHelper.SelectPlaneAndInsertSketch(swDoc))
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.CreateLineErrors.PlaneSelectFailed));
        return;
    }

    var factor = SolidWorksHelper.GetLengthConversionFactor(swDoc);

    if (!SolidWorksHelper.CreateLine(swDoc.SketchManager,
        startX * factor, startY * factor, 0,
        endX * factor, endY * factor, 0))
    {
        AnsiConsole.MarkupLine(string.Format(Constants.Errors.FailedFormat, Constants.CreateLineErrors.LineFailed));
        return;
    }

    swDoc.ClearSelection2(true);
    swDoc.ViewZoomtofit2();

    AnsiConsole.MarkupLine(Constants.CreateLineMessages.Success);
}
