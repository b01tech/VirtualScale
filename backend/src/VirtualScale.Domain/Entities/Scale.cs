namespace VirtualScale.Domain.Entities;

public class Scale(CalibrationData calibration)
{
    public List<LoadCell> LoadCells { get; init; } = new();
    public decimal RawValue { get; private set; }
    public decimal FactorCal { get; private set; } = 1.0m;
    public decimal ZeroConstant { get; private set; } = 0.0m;
    public decimal SpanConstant { get; private set; } = 0.0m;

    public decimal BruteWeight { get; private set; }
    public decimal TareWeight { get; private set; }
    public decimal NetWeight => BruteWeight - TareWeight;

    public bool IsTared { get; private set; } = false;
    public bool IsOnZero => CheckZero();
    public bool IsStable { get; private set; } = true;

    public void Tare()
    {
        if (IsTared)
        {
            TareWeight = 0.0m;
            IsTared = false;
            return;
        }
        TareWeight = BruteWeight;
        IsTared = true;
    }

    public bool CheckZero() => NetWeight == 0.0m;

    private void SetRawValue() => RawValue = LoadCells.Average(cell => cell.GetValue());

    public void CalibrateZero()
    {
        SetRawValue();
        ZeroConstant = RawValue;
    }

    public void CalibrateSpan()
    {
        SetRawValue();
        SpanConstant = RawValue;
        FactorCal = (SpanConstant - ZeroConstant) / calibration.ReferenceWeight;
    }

    public void CalcWeight()
    {
        BruteWeight = (RawValue - ZeroConstant) * FactorCal;
    }

    public string PrintData()
    {
        return $"RawValue: {RawValue}\n FactorCal: {FactorCal}\n ZeroConstant: {ZeroConstant}\n SpanConstant: {SpanConstant}\n BruteWeight: {BruteWeight}\n NetWeight: {NetWeight}\n TareWeight: {TareWeight}\n ";
    }
}
