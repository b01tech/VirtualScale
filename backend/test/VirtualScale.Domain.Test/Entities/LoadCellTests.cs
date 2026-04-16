using VirtualScale.Domain.Entities;

namespace VirtualScale.Domain.Test.Entities;

public class LoadCellTests
{
    [Fact]
    public void Constructor_SetsId()
    {
        var loadCell = new LoadCell(1);
        Assert.Equal(1, loadCell.Id);
    }

    [Fact]
    public void Constructor_DefaultsFactorToOne()
    {
        var loadCell = new LoadCell(1);
        Assert.Equal(1.0m, loadCell.Factor);
    }

    [Fact]
    public void SetFactor_UpdatesFactor()
    {
        var loadCell = new LoadCell(1);
        loadCell.SetFactor(1.5m);
        Assert.Equal(1.5m, loadCell.Factor);
    }

    [Fact]
    public void SetRawValue_UpdatesValue()
    {
        var loadCell = new LoadCell(1);
        loadCell.SetRawValue(123.456m);
        Assert.Equal(123.456m, loadCell.RawValue);
    }

    [Fact]
    public void GetValue_ReturnsRawValueMultipliedByFactor()
    {
        var loadCell = new LoadCell(1);
        loadCell.SetRawValue(100m);
        loadCell.SetFactor(1.5m);
        
        Assert.Equal(150m, loadCell.GetValue());
    }

    [Fact]
    public void GetValue_WithDefaultFactor_ReturnsRawValue()
    {
        var loadCell = new LoadCell(1);
        loadCell.SetRawValue(100m);
        
        Assert.Equal(100m, loadCell.GetValue());
    }
}
