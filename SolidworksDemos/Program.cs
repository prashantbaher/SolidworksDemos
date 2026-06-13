using Spectre.Console.Cli;
using SolidworksDemos.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("SolidworksDemos");
    config.AddCommand<HelloCommand>("hello")
        .WithDescription("Say hello to someone");
});

if (args.Length == 0)
{
    args = ["hello"];
}

return app.Run(args);
