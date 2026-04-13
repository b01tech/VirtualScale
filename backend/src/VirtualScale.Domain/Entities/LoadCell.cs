using VirtualScale.Domain.Enums;

namespace VirtualScale.Domain.Entities;

public class LoadCell(int id)
{
    public int Id { get; init; } = id;
    public decimal RawValue { get; private set; } = 0.0m;
    public decimal Factor { get; private set; } = 1.0m;
    public CellStatus Status { get; private set; } = CellStatus.Ok;

    public void SetRawValue(decimal value) => RawValue = value;

    public void SetFactor(decimal value) => Factor = value;

    public decimal GetValue() => RawValue * Factor;
}
