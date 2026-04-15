namespace VirtualScale.Domain.Entities;

public class Scale(CalibrationData calibration)
{
    public CalibrationData Calibration => calibration;
    public List<LoadCell> LoadCells { get; init; } = new();
    public int NumberOfCells { get; private set; } = 2;
    public decimal RawValue { get; private set; }
    public int FilterLevel { get; private set; } = 1;
    public decimal FactorCal { get; private set; } = 1000.0m;
    public decimal ZeroConstant { get; private set; } = 0.0m;
    public decimal SpanConstant { get; private set; } = 0.0m;

    public decimal BruteWeight { get; private set; }
    public decimal TareWeight { get; private set; } = 0.0m;
    public decimal NetWeight => BruteWeight - TareWeight;

    public bool IsTared { get; private set; } = false;
    public bool IsOnZero => CheckZero();
    public bool IsStable { get; private set; } = true;
    public bool NeedsCalibrationAdjustment { get; private set; } = false;

    private decimal? FilteredRawValue { get; set; }
    private decimal? LastBruteWeight { get; set; }
    private int StableCounter { get; set; }

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
        if (LoadCells.Count == 0)
        {
            RawValue = ApplyFilter(0.0m);
            return;
        }

        var selected = LoadCells.OrderBy(cell => cell.Id).Take(NumberOfCells).ToList();
        var raw = selected.Count == 0 ? 0.0m : selected.Average(cell => cell.GetValue());
        RawValue = ApplyFilter(raw);
    }

    public void SetFilterLevel(int level)
    {
        FilterLevel = Math.Clamp(level, 0, 10);
        FilteredRawValue = null;
        StableCounter = 0;
        IsStable = false;
    }

    private decimal ApplyFilter(decimal raw)
    {
        if (FilterLevel <= 0)
        {
            FilteredRawValue = raw;
            return raw;
        }

        var alpha = GetFilterAlpha(FilterLevel);
        FilteredRawValue = FilteredRawValue is null
            ? raw
            : FilteredRawValue.Value + alpha * (raw - FilteredRawValue.Value);
        return FilteredRawValue.Value;
    }

    private static decimal GetFilterAlpha(int level)
    {
        return level switch
        {
            1 => 0.50m,
            2 => 0.30m,
            3 => 0.20m,
            4 => 0.10m,
            5 => 0.05m,
            _ => 1.0m / (level * 5.0m + 1.0m),
        };
    }

    public void CalibrateZero()
    {
        SetRawValue();
        ZeroConstant = RawValue;
        EvaluateCalibrationAdjustmentStatus();
    }

    public void CalibrateSpan()
    {
        SetRawValue();
        SpanConstant = RawValue;
        FactorCal = (SpanConstant - ZeroConstant) / calibration.ReferenceWeight;
        EvaluateCalibrationAdjustmentStatus();
    }

    public void CalcWeight()
    {
        SetRawValue();
        BruteWeight = (RawValue - ZeroConstant) / FactorCal;
        UpdateStability();
    }

    private void UpdateStability()
    {
        var tolerance = Math.Max((decimal)calibration.Resolution, 0.000001m);
        var requiredSamples = 5;

        if (LastBruteWeight is null)
        {
            LastBruteWeight = BruteWeight;
            StableCounter = 0;
            IsStable = false;
            return;
        }

        var delta = Math.Abs(BruteWeight - LastBruteWeight.Value);
        if (delta <= tolerance)
        {
            StableCounter++;
        }
        else
        {
            StableCounter = 0;
        }

        LastBruteWeight = BruteWeight;
        IsStable = StableCounter >= requiredSamples;
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
            loadCell = new LoadCell(id);
            LoadCells.Add(loadCell);
        }

        loadCell.SetFactor(factor);
        return true;
    }

    public void ResetLoadCellFactors()
    {
        foreach (var loadCell in LoadCells)
        {
            loadCell.SetFactor(1.0m);
        }
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

    public void SetNumberOfCells(int numberOfCells)
    {
        NumberOfCells = Math.Clamp(numberOfCells, 1, 32);
    }

    public void UpdateCalibrationSettings(
        int numberOfCells,
        decimal capMax,
        int division,
        int decimalPlaces,
        decimal referenceWeight
    )
    {
        SetNumberOfCells(numberOfCells);
        calibration.Update(capMax, division, decimalPlaces, referenceWeight);
        NeedsCalibrationAdjustment = true;
    }

    private void EvaluateCalibrationAdjustmentStatus()
    {
        if (SpanConstant > ZeroConstant)
        {
            NeedsCalibrationAdjustment = false;
        }
    }

    private decimal RoundValue(decimal value) =>
        Math.Round(value / (decimal)calibration.Resolution) * (decimal)calibration.Resolution;
}
