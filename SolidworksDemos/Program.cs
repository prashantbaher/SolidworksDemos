using System;
using Spectre.Console;
using SolidworksDemos;

AnsiConsole.Write(new Rule("[yellow]SolidworksDemos[/]").RuleStyle("yellow"));

var category = MenuBuilder.SelectCategory();
var topic = MenuBuilder.SelectTopic(category);
var variant = MenuBuilder.SelectVariant(topic);

var action = ActionFactory.Create(category, topic, variant);
action.Execute();

AnsiConsole.MarkupLine("[dim]Press any key to exit...[/]");
Console.ReadKey();
