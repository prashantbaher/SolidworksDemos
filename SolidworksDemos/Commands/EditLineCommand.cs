using Spectre.Console;
using Spectre.Console.Cli;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Services;

namespace SolidworksDemos.Commands;

public class EditLineCommand : Command<EditLineCommand.Settings>
{
    private readonly ISolidWorksService _service;

    public class Settings : CommandSettings
    {
    }

    public EditLineCommand()
    {
        _service = new SolidWorksService();
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var messageToShow = string.Empty;

        AnsiConsole.Status()
            .Start(Constants.EditLineMessages.Connecting, ctx =>
            {
                if (!_service.CreateSolidworksInstance(out var swApp, out messageToShow))
                    return;

                if (!_service.GetActiveDocument(swApp, out var swDoc, out messageToShow))
                    return;

                if (!_service.GetActiveSketch(swDoc, out var sketch, out messageToShow))
                    return;

                ctx.Status(Constants.EditLineMessages.FindingSketch);

                if (!_service.FindFirstLine(sketch, out var line, out var startPt, out var endPt, out messageToShow))
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

                if (!_service.UpdateSketchPoint(point, newX, newY, out messageToShow))
                    return;

                if (!_service.RebuildAndZoom(swDoc, out messageToShow))
                    return;

                messageToShow = string.Format(Constants.EditLineMessages.SuccessFormat,
                    pointName, oldX, oldY, newX, newY);
            });

        if (!string.IsNullOrEmpty(messageToShow))
        {
            AnsiConsole.MarkupLine(messageToShow.Contains("Failed") || messageToShow.Contains("failed")
                ? string.Format(Constants.Errors.FailedFormat, messageToShow)
                : messageToShow);
        }

        return messageToShow.Contains("Failed") || messageToShow.Contains("failed") ? 1 : 0;
    }
}
