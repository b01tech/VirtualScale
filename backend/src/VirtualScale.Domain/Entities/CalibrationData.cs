namespace VirtualScale.Domain.Entities;

public class CalibrationData
{
    public decimal CapMax { get; private set; }
    public int Division { get; private set; }
    public int DecimalPlaces { get; private set; }
    public float Resolution { get; private set; }
    public decimal CapMin { get; private set; }
    public decimal ReferenceWeight { get; private set; }

    public CalibrationData(decimal capMax, int division, int decimalPlaces, decimal referenceWeight)
    {
        CapMax = capMax;
        Division = division;
        DecimalPlaces = decimalPlaces;
        ReferenceWeight = referenceWeight;
        Resolution = CalcResolution();
        CapMin = CalcCapMin();
    }

    private float CalcResolution() => Division / (float)Math.Pow(10, DecimalPlaces);

    private decimal CalcCapMin() => (decimal)(Resolution * 20);
}
