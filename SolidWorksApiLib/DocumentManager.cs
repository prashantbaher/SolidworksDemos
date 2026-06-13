using SolidWorksApiLib.Models;

namespace SolidWorksApiLib;

public class DocumentManager
{
    private readonly SolidWorksConnector _connector;

    public DocumentManager(SolidWorksConnector connector)
    {
        _connector = connector;
    }

    public SwDocumentInfo? GetActiveDocument()
    {
        var swApp = _connector.SwApp;
        if (swApp == null) return null;

        try
        {
            dynamic? activeDoc = swApp.ActiveDoc;
            if (activeDoc == null) return null;

            return new SwDocumentInfo
            {
                Name = activeDoc.GetTitle(),
                Path = activeDoc.GetPathName() ?? "",
                Type = GetDocumentType(activeDoc),
                IsOpened = true
            };
        }
        catch
        {
            return null;
        }
    }

    public SwDocumentInfo? OpenDocument(string filePath)
    {
        var swApp = _connector.SwApp;
        if (swApp == null) return null;

        try
        {
            int docType = GetDocTypeFromExtension(filePath);
            dynamic? doc = swApp.OpenDoc6(
                filePath, docType, 0, "", 0, 0);

            if (doc == null) return null;

            return new SwDocumentInfo
            {
                Name = doc.GetTitle(),
                Path = filePath,
                Type = GetDocumentType(doc),
                IsOpened = true
            };
        }
        catch
        {
            return null;
        }
    }

    public void CloseActiveDocument()
    {
        var swApp = _connector.SwApp;
        if (swApp == null) return;

        try
        {
            dynamic? activeDoc = swApp.ActiveDoc;
            if (activeDoc != null)
            {
                activeDoc.CloseDoc();
            }
        }
        catch { }
    }

    private static string GetDocumentType(dynamic doc)
    {
        try
        {
            var path = (string)doc.GetPathName();
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext switch
            {
                ".sldprt" => "Part",
                ".sldasm" => "Assembly",
                ".slddrw" => "Drawing",
                _ => "Unknown"
            };
        }
        catch
        {
            return "Unknown";
        }
    }

    private static int GetDocTypeFromExtension(string filePath)
    {
        var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
        return ext switch
        {
            ".sldprt" => 1,    // swDocPART
            ".sldasm" => 2,    // swDocASSEMBLY
            ".slddrw" => 3,    // swDocDRAWING
            _ => 1
        };
    }
}
