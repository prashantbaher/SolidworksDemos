using Spectre.Console;
using Spectre.Console.Cli;

namespace SolidworksDemos.Commands;

public class HelloCommand : Command<HelloCommand.Settings>
{
    public class Settings : CommandSettings
    {
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var name = AnsiConsole.Ask<string>("What's your [green]name[/]?");
        AnsiConsole.MarkupLine($"[bold green]Hello, {name}![/]");
        return 0;
    }
}
