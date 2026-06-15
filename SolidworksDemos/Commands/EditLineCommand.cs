using Spectre.Console;
using Spectre.Console.Cli;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Commands;

public class EditLineCommand : Command<EditLineCommand.Settings>
{
    private readonly IApplicationService _appService;
    private readonly IDocumentService _docService;
    private readonly ISketchService _sketchService;
    private readonly ILineService _lineService;

    public class Settings : CommandSettings
    {
    }

    public EditLineCommand()
    {
        _appService = new Services.ApplicationService();
        _docService = new Services.DocumentService();
        _sketchService = new Services.SketchService();
        _lineService = new Services.LineService();
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var messageToShow = string.Empty;

        AnsiConsole.Status()
            .Start(Constants.EditLineMessages.Connecting, ctx =>
            {
                if (!_appService.CreateInstance(out var swApp, out messageToShow))
                    return;

                if (!_docService.GetActiveDocument(swApp, out var swDoc, out messageToShow))
                    return;

                if (!_sketchService.GetActiveSketch(swDoc, out var sketch, out messageToShow))
                    return;

                ctx.Status(Constants.EditLineMessages.FindingSketch);

                if (!_lineService.FindFirstLine(sketch, out var line, out var startPt, out var endPt, out messageToShow))
                    return;

                var startX = (double)startPt.X;
                var startY = (double)startPt.Y;
                var endX = (double)endPt.X;
                var endY = (double)endPt.Y;

                AnsiConsole.MarkupLine(Constants.EditLineMessages.PointInfoFormat, startX, startY, endX, endY);

                var pointChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(Constants.EditLineMessages.PointSelectionTitle)
                        .AddChoices(Constants.EditLineMessages.StartPoint, Constants.EditLineMessages.EndPoint));

                var newX = AnsiConsole.Ask<double>(Constants.EditLineMessages.PromptX);
                var newY = AnsiConsole.Ask<double>(Constants.EditLineMessages.PromptY);

                var point = pointChoice == Constants.EditLineMessages.StartPoint ? startPt : endPt;
                var pointName = pointChoice == Constants.EditLineMessages.StartPoint ? "Start" : "End";
                var oldX = pointChoice == Constants.EditLineMessages.StartPoint ? startX : endX;
                var oldY = pointChoice == Constants.EditLineMessages.StartPoint ? startY : endY;

                if (!_lineService.UpdatePoint(point, newX, newY, out messageToShow))
                    return;

                if (!_docService.RebuildAndZoom(swDoc, out messageToShow))
                    return;

                messageToShow = string.Format(Constants.EditLineMessages.SuccessFormat,
                    pointName, oldX, oldY, newX, newY);
            });

        if (!string.IsNullOrEmpty(messageToShow))
        {
            var isError = messageToShow.Contains("Failed", StringComparison.OrdinalIgnoreCase);
            AnsiConsole.MarkupLine(isError
                ? string.Format(Constants.Errors.FailedFormat, messageToShow)
                : messageToShow);
            return isError ? 1 : 0;
        }

        return 0;
    }
}
