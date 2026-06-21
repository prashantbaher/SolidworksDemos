using SolidworksDemos.Constants;
using SolidworksDemos.Sketches;
using System;
using Xunit;

namespace SolidworksDemos.Tests;

public class ActionFactoryTests
{
    [Fact]
    public void Create_SketchesLineCreate_ReturnsCreateLine()
    {
        var action = ActionFactory.Create(Menu.Documents.Sketches, Menu.SketchArticles.Line, Menu.ArticleTypes.Create);

        Assert.IsType<CreateLine>(action);
    }

    [Fact]
    public void Create_SketchesLineEdit_ReturnsEditSketchLine()
    {
        var action = ActionFactory.Create(Menu.Documents.Sketches, Menu.SketchArticles.Line, Menu.ArticleTypes.Edit);

        Assert.IsType<EditSketchLine>(action);
    }

    [Fact]
    public void Create_InvalidCombination_ThrowsNotSupportedException()
    {
        var ex = Assert.Throws<NotSupportedException>(() =>
            ActionFactory.Create("Invalid", "Invalid", "Invalid"));

        Assert.Contains("Invalid", ex.Message);
    }
}
