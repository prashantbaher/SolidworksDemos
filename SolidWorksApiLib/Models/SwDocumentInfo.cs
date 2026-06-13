namespace SolidWorksApiLib.Models;

public class SwDocumentInfo
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsOpened { get; set; }
}
