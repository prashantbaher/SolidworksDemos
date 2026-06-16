using System;
using Spectre.Console;
using SolidworksDemos.Constants;

namespace SolidworksDemos;

public static class MenuBuilder
{
    public static string SelectCategory()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{Menu.CategoryPrompt}[/]")
                .AddChoices(Menu.Sketches, Menu.PartFeatures, Menu.AssemblyFeatures,
                    Menu.DrawingFeatures, Menu.EquationManager));
    }

    public static string SelectTopic(string category)
    {
        string[] topics = category switch
        {
            Menu.Sketches => new[] { Menu.Line, Menu.Point, Menu.CenterLine, Menu.Rectangle },
            _ => Array.Empty<string>()
        };

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{Menu.TopicPrompt}[/]")
                .AddChoices(topics));
    }

    public static string SelectVariant(string topic)
    {
        string[] variants = topic switch
        {
            Menu.Line => new[] { Menu.Create, Menu.Edit },
            Menu.Point => new[] { Menu.Create },
            _ => Array.Empty<string>()
        };

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{Menu.VariantPrompt}[/]")
                .AddChoices(variants));
    }
}
