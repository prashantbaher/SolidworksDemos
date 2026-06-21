namespace SolidworksDemos.Constants.Sketches;

public static class LineMessages
{
    public static class Prompts
    {
        public const string StartPointX = "Enter Start Point [green]X[/] (mm):";
        public const string StartPointY = "Enter Start Point [green]Y[/] (mm):";
        public const string EndPointX = "Enter End Point [green]X[/] (mm):";
        public const string EndPointY = "Enter End Point [green]Y[/] (mm):";
        public const string NewStartPointX = "Enter New Start Point [green]X[/] (mm):";
        public const string NewStartPointY = "Enter New Start Point [green]Y[/] (mm):";
        public const string NewEndPointX = "Enter New End Point [green]X[/] (mm):";
        public const string NewEndPointY = "Enter New End Point [green]Y[/] (mm):";
        public const string EditMode = "Select [green]edit mode[/]";
        public const string NewSwInstance = "New SW Instance";
        public const string RunningSwInstance = "Running SW Instance";
    }

    public static class Headers
    {
        public const string StartPoint = "Start Point";
        public const string EndPoint = "End Point";
        public const string NewStartPoint = "New Start Point";
        public const string NewEndPoint = "New End Point";
    }

    public static class Results
    {
        public const string SwAppNotFound = "[red]Failed to find SolidWorks application.[/]";
        public const string CreatePartFailed = "[red]Failed to create new part document.[/]";
        public const string SelectPlaneFailed = "[red]Failed to select Right Plane.[/]";
        public const string CreateLineFailed = "[red]Failed to create sketch line.[/]";
        public const string NoRunningSwInstance = "[red]No running SolidWorks instance found.[/]";
        public const string NoActiveDocument = "[red]No active document open.[/]";
        public const string NotPartDocument = "[red]Active document is not a part document.[/]";
        public const string NoActiveSketch = "[red]No active sketch found. Enter sketch mode first.[/]";
        public const string NoSketchSegments = "[red]No sketch segments found.[/]";
        public const string NoLineFound = "[red]No line found in the active sketch.[/]";
        public const string LineCreated = "[green]Sketch line successfully created.[/]";
        public const string LineEdited = "[green]Sketch line successfully edited.[/]";
    }
}
