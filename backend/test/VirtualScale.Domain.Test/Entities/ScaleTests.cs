using VirtualScale.Domain.Entities;

namespace VirtualScale.Domain.Test.Entities;

public class ScaleTests
{
    private readonly Scale _scale;

    public ScaleTests()
    {
        var calibration = new CalibrationData(10, 2, 3, 5);
        _scale = new Scale(calibration);
    }

    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        Assert.NotNull(_scale.Calibration);
        Assert.Equal(2, _scale.NumberOfCells);
        Assert.Equal(1, _scale.FilterLevel);
        Assert.Equal(1000m, _scale.FactorCal);
        
        var (zero, _, _, _) = _scale.GetCalibrationData();
        Assert.Equal(0m, zero);
    }

    [Fact]
    public void SetNumberOfCells_ClampsValue()
    {
        _scale.SetNumberOfCells(0);
        Assert.Equal(1, _scale.NumberOfCells);

        _scale.SetNumberOfCells(100);
        Assert.Equal(32, _scale.NumberOfCells);

        _scale.SetNumberOfCells(4);
        Assert.Equal(4, _scale.NumberOfCells);
    }

    [Fact]
    public void SetFilterLevel_ClampsValue()
    {
        _scale.SetFilterLevel(-1);
        Assert.Equal(0, _scale.FilterLevel);

        _scale.SetFilterLevel(100);
        Assert.Equal(10, _scale.FilterLevel);

        _scale.SetFilterLevel(5);
        Assert.Equal(5, _scale.FilterLevel);
    }

    [Fact]
    public void Tare_SetsTareWeight()
    {
        _scale.UpdateLoadCell(1, 50000m);
        _scale.CalibrateZero();

        _scale.UpdateLoadCell(1, 100000m);
        _scale.CalcWeight();

        _scale.Tare();

        Assert.True(_scale.IsTared);
        Assert.Equal(0m, _scale.NetWeight);
    }

    [Fact]
    public void Tare_WhenAlreadyTared_Untares()
    {
        _scale.UpdateLoadCell(1, 50000m);
        _scale.CalibrateZero();

        _scale.UpdateLoadCell(1, 100000m);
        _scale.CalcWeight();

        _scale.Tare();
        var result = _scale.Tare();

        Assert.True(result);
        Assert.False(_scale.IsTared);
    }

    [Fact]
    public void UpdateCalibrationSettings_MarksCalibrationNeeded()
    {
        _scale.UpdateCalibrationSettings(2, "kg", 20, 5, 2, 10);
        
        Assert.True(_scale.NeedsCalibrationAdjustment);
        Assert.Equal("kg", _scale.Calibration.Unit);
    }

    [Fact]
    public void UpdateLoadCell_CreatesNewLoadCell()
    {
        _scale.UpdateLoadCell(1, 50000m);

        Assert.Single(_scale.LoadCells);
        Assert.Equal(1, _scale.LoadCells[0].Id);
    }

    [Fact]
    public void UpdateLoadCell_UpdatesExistingLoadCell()
    {
        _scale.UpdateLoadCell(1, 50000m);
        _scale.UpdateLoadCell(1, 60000m);

        Assert.Single(_scale.LoadCells);
        Assert.Equal(60000m, _scale.LoadCells[0].RawValue);
    }

    [Fact]
    public void SetLoadCellFactor_UpdatesFactor()
    {
        _scale.UpdateLoadCell(1, 50000m);
        _scale.SetLoadCellFactor(1, 1.5m);

        Assert.Equal(1.5m, _scale.LoadCells[0].Factor);
    }

    [Fact]
    public void ResetLoadCellFactors_SetsAllToOne()
    {
        _scale.UpdateLoadCell(1, 50000m);
        _scale.UpdateLoadCell(2, 50000m);
        _scale.SetLoadCellFactor(1, 1.5m);
        _scale.SetLoadCellFactor(2, 0.8m);

        _scale.ResetLoadCellFactors();

        Assert.Equal(1.0m, _scale.LoadCells[0].Factor);
        Assert.Equal(1.0m, _scale.LoadCells[1].Factor);
    }

    [Fact]
    public void LoadCalibrationData_SetsValues()
    {
        _scale.LoadCalibrationData(50000m, 150000m, 100m, 3);

        var (zero, span, factor, filter) = _scale.GetCalibrationData();

        Assert.Equal(50000m, zero);
        Assert.Equal(150000m, span);
        Assert.Equal(100m, factor);
        Assert.Equal(3, filter);
    }

    [Fact]
    public void GetCalibrationData_ReturnsValues()
    {
        _scale.LoadCalibrationData(50000m, 150000m, 100m, 3);

        var (zero, span, factor, filter) = _scale.GetCalibrationData();

        Assert.Equal(50000m, zero);
        Assert.Equal(150000m, span);
        Assert.Equal(100m, factor);
        Assert.Equal(3, filter);
    }
}
