using Spectre.Console;
using Spectre.Console.Cli;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Commands;

public class CreateLineCommand : Command<CreateLineCommand.Settings>
{
    private readonly IApplicationService _appService;
    private readonly IDocumentService _docService;
    private readonly ISketchService _sketchService;
    private readonly ILineService _lineService;

    public class Settings : CommandSettings
    {
    }

    public CreateLineCommand()
    {
        _appService = new Services.ApplicationService();
        _docService = new Services.DocumentService();
        _sketchService = new Services.SketchService();
        _lineService = new Services.LineService();
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var messageToShow = string.Empty;

        var startX = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptStartX);
        var startY = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptStartY);
        var endX = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptEndX);
        var endY = AnsiConsole.Ask<double>(Constants.CreateLineMessages.PromptEndY);

        AnsiConsole.Status()
            .Start(Constants.CreateLineMessages.Creating, ctx =>
            {
                if (!_docService.CreatePartDocument(out var swApp, out var swDoc, out messageToShow))
                    return;

                if (!_sketchService.SelectPlaneAndInsertSketch(swDoc, out messageToShow))
                    return;

                var factor = _docService.GetLengthConversionFactor(swDoc);
                var x1 = startX * factor;
                var y1 = startY * factor;
                var x2 = endX * factor;
                var y2 = endY * factor;

                if (!_lineService.CreateLine(swDoc.SketchManager, x1, y1, 0, x2, y2, 0, out messageToShow))
                    return;

                swDoc.ClearSelection2(true);
                swDoc.ViewZoomtofit2();

                messageToShow = Constants.CreateLineMessages.Success;
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
