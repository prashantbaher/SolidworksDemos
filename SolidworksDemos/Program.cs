using Spectre.Console.Cli;
using SolidworksDemos.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SolidworksDemos");
    config.AddCommand<HelloCommand>("hello")
        .WithDescription("Say hello to someone");
});

return app.Run(args.Length == 0 ? ["hello"] : args);
