using VirtualScale.Domain.Entities;

namespace VirtualScale.Domain.Test.Entities;

public class CalibrationDataTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var calibration = new CalibrationData(10, 2, 3, 5);

        Assert.Equal(10m, calibration.CapMax);
        Assert.Equal(2, calibration.Division);
        Assert.Equal(3, calibration.DecimalPlaces);
        Assert.Equal(5m, calibration.ReferenceWeight);
        Assert.Equal("kg", calibration.Unit);
    }

    [Theory]
    [InlineData("kg", "kg")]
    [InlineData("KG", "kg")]
    [InlineData("g", "g")]
    [InlineData("G", "g")]
    [InlineData("invalid", "kg")]
    public void Update_NormalizesUnit(string input, string expected)
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        calibration.Update(input, 10, 2, 3, 5);
        Assert.Equal(expected, calibration.Unit);
    }

    [Fact]
    public void Update_CalculatesResolution()
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        
        var expectedResolution = 2 / (float)Math.Pow(10, 3);
        Assert.Equal(expectedResolution, calibration.Resolution);
    }

    [Fact]
    public void Update_ConvertsGramsToKg()
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        calibration.Update("g", 10000, 2, 0, 5000);

        Assert.Equal("g", calibration.Unit);
        Assert.Equal(10m, calibration.CapMax);
        Assert.Equal(5m, calibration.ReferenceWeight);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(100, 100)]
    public void Update_ClampsDivision(int input, int expected)
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        calibration.Update("kg", 10, input, 3, 5);
        Assert.Equal(expected, calibration.Division);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(7, 6)]
    [InlineData(3, 3)]
    public void Update_ClampsDecimalPlaces(int input, int expected)
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        calibration.Update("kg", 10, 2, input, 5);
        Assert.Equal(expected, calibration.DecimalPlaces);
    }

    [Fact]
    public void Update_CalculatesCapMin()
    {
        var calibration = new CalibrationData(10, 1, 3, 5);
        
        var expectedCapMin = (decimal)(calibration.Resolution * 20);
        Assert.Equal(expectedCapMin, calibration.CapMin);
    }

    [Fact]
    public void Update_PreventsZeroCapMax()
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        calibration.Update("kg", 0, 2, 3, 5);
        
        Assert.True(calibration.CapMax > 0);
    }
}
