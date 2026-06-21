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
                .Title($"[yellow]{Menu.Prompts.CategoryPrompt}[/]")
                .AddChoices(Menu.Documents.Sketches, Menu.Documents.PartFeatures, Menu.Documents.AssemblyFeatures,
                    Menu.Documents.DrawingFeatures, Menu.Documents.EquationManager));
    }

    public static string SelectTopic(string category)
    {
        string[] topics = category switch
        {
            Menu.Documents.Sketches => new[] { Menu.SketchArticles.Line, Menu.SketchArticles.Point, Menu.SketchArticles.CenterLine, Menu.SketchArticles.Rectangle },
            _ => Array.Empty<string>()
        };

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{Menu.Prompts.TopicPrompt}[/]")
                .AddChoices(topics));
    }

    public static string SelectVariant(string topic)
    {
        string[] variants = topic switch
        {
            Menu.SketchArticles.Line => new[] { Menu.ArticleTypes.Create, Menu.ArticleTypes.Edit },
            Menu.SketchArticles.Point => new[] { Menu.ArticleTypes.Create },
            _ => Array.Empty<string>()
        };

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[yellow]{Menu.Prompts.VariantPrompt}[/]")
                .AddChoices(variants));
    }
}
