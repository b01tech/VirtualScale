using VirtualScale.Domain.Entities;
using VirtualScale.Worker.Configuration;

namespace VirtualScale.Worker.Services;

public static class InitializationService
{
    public static void Initialize(Scale scale)
    {
        for (int i = 1; i <= AppConfiguration.NumberLoadCells; i++)
        {
            scale.LoadCells.Add(new LoadCell(i));
        }
    }
}
