using System;
using SolidworksDemos;
using SolidworksDemos.Interfaces;
using SolidworksDemos.Models;
using SolidworksDemos.Helpers;

Console.WriteLine("=== Create Sketch Line ===");

Console.Write("Enter Start X (mm): ");
double startX = double.Parse(Console.ReadLine());

Console.Write("Enter Start Y (mm): ");
double startY = double.Parse(Console.ReadLine());

Console.Write("Enter End X (mm): ");
double endX = double.Parse(Console.ReadLine());

Console.Write("Enter End Y (mm): ");
double endY = double.Parse(Console.ReadLine());

var startPoint = new PointViewModel
{
    Header = "Start Point",
    XPoint = startX,
    YPoint = startY,
    ZPoint = 0
};

var endPoint = new PointViewModel
{
    Header = "End Point",
    XPoint = endX,
    YPoint = endY,
    ZPoint = 0
};

var conversionHelper = new UnitConversionHelper();
var lineCreator = new LineCreator
{
    StartPointViewModel = startPoint,
    EndPointViewModel = endPoint,
    conversionHelper = conversionHelper
};

Console.WriteLine("Creating sketch line in SolidWorks...");

if (lineCreator.CreateLineMethod())
{
    Console.WriteLine("Success: " + lineCreator.messageToShow);
}
else
{
    Console.WriteLine("Error: " + lineCreator.messageToShow);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
