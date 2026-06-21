using System;
using SolidworksDemos.Constants;
using SolidworksDemos.Interfaces;

namespace SolidworksDemos;

public static class ActionFactory
{
    public static ICrudAction Create(string category, string topic, string variant)
    {
        return (category, topic, variant) switch
        {
            (Menu.Documents.Sketches, Menu.SketchArticles.Line, Menu.ArticleTypes.Create) => new Sketches.CreateLine(new Helpers.SwHelper()),
            (Menu.Documents.Sketches, Menu.SketchArticles.Line, Menu.ArticleTypes.Edit) => new Sketches.EditSketchLine(new Helpers.SwHelper()),
            _ => throw new NotSupportedException($"No action for {category} > {topic} > {variant}")
        };
    }
}
