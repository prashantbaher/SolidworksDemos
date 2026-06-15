namespace SolidworksDemos;

public static class Constants
{
    public static class Menu
    {
        public const string Title = "What would you like to do?";
        public const string HelloOption = "Hello";
        public const string EditLineOption = "Edit Sketch Line";
        public const string CreateLineOption = "Create Line";
        public const string ExitOption = "Exit";
    }

    public static class Commands
    {
        public const string Hello = "hello";
        public const string EditLine = "edit-line";
        public const string CreateLine = "create-line";
    }

    public static class HelloMessages
    {
        public const string NamePrompt = "What's your [green]name[/]?";
        public const string GreetingTemplate = "[bold green]Hello, {0}![/] [yellow]:wave:[/]";
    }

    public static class EditLineMessages
    {
        public const string Connecting = "Connecting to SolidWorks...";
        public const string FindingSketch = "Finding active sketch...";
        public const string PointSelectionTitle = "Which point do you want to update?";
        public const string StartPoint = "Start Point";
        public const string EndPoint = "End Point";
        public const string PromptX = "Enter new X:";
        public const string PromptY = "Enter new Y:";
        public const string SuccessFormat = "[green]:check_mark: {0} point updated from ({1}, {2}) to ({3}, {4})[/]";
        public const string PointInfoFormat = "[yellow]Start: ({0}, {1})  End: ({2}, {3})[/]";
    }

    public static class CreateLineMessages
    {
        public const string Title = "Create a new sketch line";
        public const string Creating = "Creating sketch line in SolidWorks...";
        public const string PromptStartX = "Enter start point [green]X[/] (mm):";
        public const string PromptStartY = "Enter start point [green]Y[/] (mm):";
        public const string PromptEndX = "Enter end point [green]X[/] (mm):";
        public const string PromptEndY = "Enter end point [green]Y[/] (mm):";
        public const string Success = "[green]:check_mark: Sketch line created successfully![/]";
    }

    public static class CreateLineErrors
    {
        public const string TemplateEmpty = "Default part template is empty.";
        public const string DocFailed = "Failed to create new part document.";
        public const string PlaneSelectFailed = "Failed to select Right Plane.";
        public const string LineFailed = "Failed to create sketch line.";
    }

    public static class Errors
    {
        public const string SolidworksFailed = "Failed to find SolidWorks application.";
        public const string NoActiveDoc = "Failed to get active document.";
        public const string NoActiveSketch = "No active sketch found.";
        public const string NoSegmentsFound = "No sketch segments found.";
        public const string NoLineFound = "No line segment found in active sketch.";
        public const string UpdateFailed = "Failed to update point coordinates.";
        public const string RebuildFailed = "Failed to rebuild model.";
        public const string FailedFormat = "[red]:cross_mark: {0}[/]";
    }
}
