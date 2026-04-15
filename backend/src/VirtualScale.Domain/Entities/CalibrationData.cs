namespace VirtualScale.Domain.Entities;

public class CalibrationData
{
    public decimal CapMax { get; private set; }
    public int Division { get; private set; }
    public int DecimalPlaces { get; private set; }
    public float Resolution { get; private set; }
    public decimal CapMin { get; private set; }
    public decimal ReferenceWeight { get; private set; }
    public string Unit { get; private set; } = "kg";

    public CalibrationData(decimal capMax, int division, int decimalPlaces, decimal referenceWeight)
    {
        Update("kg", capMax, division, decimalPlaces, referenceWeight);
    }

    public void Update(string? unit, decimal capMax, int division, int decimalPlaces, decimal referenceWeight)
    {
        Unit = NormalizeUnit(unit);

        var capMaxKg = Unit == "g" ? capMax / 1000.0m : capMax;
        var referenceWeightKg = Unit == "g" ? referenceWeight / 1000.0m : referenceWeight;

        CapMax = Math.Max(capMaxKg, 0.000001m);
        Division = Math.Max(division, 1);
        DecimalPlaces = Math.Clamp(decimalPlaces, 0, 6);
        ReferenceWeight = Math.Max(referenceWeightKg, 0.000001m);
        Resolution = CalcResolution();
        CapMin = CalcCapMin();
    }

    private static string NormalizeUnit(string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized == "g" ? "g" : "kg";
    }

    private float CalcResolution()
    {
        var baseResolution = Division / (float)Math.Pow(10, DecimalPlaces);
        return Unit == "g" ? baseResolution / 1000.0f : baseResolution;
    }

    private decimal CalcCapMin() => (decimal)(Resolution * 20);
}
