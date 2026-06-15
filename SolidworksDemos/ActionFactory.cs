using System;
using SolidworksDemos.Abstractions;
using SolidworksDemos.Constants;

namespace SolidworksDemos;

public static class ActionFactory
{
    public static ICrudAction Create(string category, string topic, string variant)
    {
        return (category, topic, variant) switch
        {
            (Menu.Sketches, Menu.Line, Menu.Create) => new Sketches.CreateLine(),
            _ => throw new NotSupportedException($"No action for {category} > {topic} > {variant}")
        };
    }
}
