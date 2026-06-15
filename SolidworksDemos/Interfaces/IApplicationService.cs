using SldWorks;

namespace SolidworksDemos.Interfaces;

public interface IApplicationService
{
    bool CreateInstance(out SldWorks.SldWorks swApp, out string message);
}
