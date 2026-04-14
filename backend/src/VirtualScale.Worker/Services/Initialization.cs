using VirtualScale.Domain.Entities;

namespace VirtualScale.Worker.Services;

public static class InitializationService
{
    public static void Initialize(Scale scale)
    {
        for (int i = 1; i <= scale.NumberOfCells; i++)
        {
            scale.LoadCells.Add(new LoadCell(i));
        }
    }
}
