using Spectre.Console;
using Spectre.Console.Cli;
using SolidworksDemos;
using SolidworksDemos.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SolidworksDemos");
    config.AddCommand<HelloCommand>(Constants.Commands.Hello)
        .WithDescription("Say hello to someone");
    config.AddCommand<EditLineCommand>(Constants.Commands.EditLine)
        .WithDescription("Edit a sketch line in SolidWorks");
    config.AddCommand<CreateLineCommand>(Constants.Commands.CreateLine)
        .WithDescription("Create a new sketch line in SolidWorks");
});

if (args.Length == 0)
{
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
            return 0;
        }

        var runArgs = choice switch
        {
            _ when choice == Constants.Menu.HelloOption => new[] { Constants.Commands.Hello },
            _ when choice == Constants.Menu.EditLineOption => new[] { Constants.Commands.EditLine },
            _ when choice == Constants.Menu.CreateLineOption => new[] { Constants.Commands.CreateLine },
            _ => new[] { Constants.Commands.Hello }
        };

        app.Run(runArgs);
        AnsiConsole.WriteLine();
    }
}

return app.Run(args);
