namespace VirtualScale.Domain.Entities;

public class Scale(CalibrationData calibration)
{
    public List<LoadCell> LoadCells { get; init; } = new();
    public int NumberOfCells { get; private set; } = 4;
    public decimal RawValue { get; private set; }
    public decimal FactorCal { get; private set; } = 1.0m;
    public decimal ZeroConstant { get; private set; } = 0.0m;
    public decimal SpanConstant { get; private set; } = 0.0m;

    public decimal BruteWeight { get; private set; }
    public decimal TareWeight { get; private set; } = 0.0m;
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

    private void SetRawValue()
    {
        RawValue = LoadCells.Count == 0 ? 0.0m : LoadCells.Average(cell => cell.GetValue());
    }

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
        SetRawValue();
        BruteWeight = (RawValue - ZeroConstant) / FactorCal;
    }

    public void UpdateLoadCell(int id, decimal value)
    {
        var loadCell = LoadCells.FirstOrDefault(c => c.Id == id);

        if (loadCell == null)
        {
            loadCell = new LoadCell(id);
            LoadCells.Add(loadCell);
        }

        loadCell.SetRawValue(value);
    }

    public bool SetLoadCellFactor(int id, decimal factor)
    {
        var loadCell = LoadCells.FirstOrDefault(c => c.Id == id);

        if (loadCell == null)
        {
            return false;
        }

        loadCell.SetFactor(factor);
        return true;
    }

    public (decimal bruteWeight, decimal netWeight, decimal tareWeight) GetRoundedWeights()
    {
        return (RoundValue(BruteWeight), RoundValue(NetWeight), RoundValue(TareWeight));
    }

    public string PrintData()
    {
        var bruteWeight = RoundValue(BruteWeight);
        var netWeight = RoundValue(NetWeight);
        var tareWeight = RoundValue(TareWeight);

        return $"RawValue: {RawValue}\n FactorCal: {FactorCal}\n ZeroConstant: {ZeroConstant}\n SpanConstant: {SpanConstant}\n BruteWeight: {bruteWeight}\n NetWeight: {netWeight}\n TareWeight: {tareWeight}\n ";
    }

    public void SetNumberOfCells(int numberOfCells) => NumberOfCells = numberOfCells;

    private decimal RoundValue(decimal value) =>
        Math.Round(value / (decimal)calibration.Resolution) * (decimal)calibration.Resolution;
}
