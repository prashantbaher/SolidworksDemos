using SolidworksDemos.Interfaces;

namespace SolidworksDemos.Models;

public class PointViewModel : IPointViewModel
{
    public string Header { get; set; }
    public double XPoint { get; set; }
    public double YPoint { get; set; }
    public double ZPoint { get; set; }
}
