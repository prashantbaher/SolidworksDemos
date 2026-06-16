using System;
using SolidworksDemos;
using SolidworksDemos.Abstractions;
using SolidworksDemos.Constants;
using SolidworksDemos.Sketches;
using Xunit;

namespace SolidworksDemos.Tests;

public class ActionFactoryTests
{
    [Fact]
    public void Create_SketchesLineCreate_ReturnsCreateLine()
    {
        var action = ActionFactory.Create(Menu.Sketches, Menu.Line, Menu.Create);

        Assert.IsType<CreateLine>(action);
    }

    [Fact]
    public void Create_SketchesLineEdit_ReturnsEditSketchLine()
    {
        var action = ActionFactory.Create(Menu.Sketches, Menu.Line, Menu.Edit);

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
