using Moq;
using VirtualScale.Api.Infrastructure.Persistence.Entities;
using VirtualScale.Api.Infrastructure.Repositories;
using VirtualScale.Api.Infrastructure.Services;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Test.Infrastructure.Services;

public class CalibrationPersistenceServiceTests
{
    private readonly Mock<ICalibrationRepository> _mockRepository;
    private readonly CalibrationPersistenceService _service;
    private readonly Scale _scale;

    public CalibrationPersistenceServiceTests()
    {
        _mockRepository = new Mock<ICalibrationRepository>();
        _service = new CalibrationPersistenceService(_mockRepository.Object);
        _scale = new Scale(new CalibrationData(10, 2, 3, 5));
    }

    [Fact]
    public async Task LoadCalibrationAsync_RecordExists_LoadsData()
    {
        var record = new CalibrationRecord
        {
            Name = "default",
            NumberOfCells = 4,
            Unit = "kg",
            CapMax = 20m,
            Division = 5,
            DecimalPlaces = 2,
            ReferenceWeight = 10m,
            ZeroConstant = 50000m,
            SpanConstant = 150000m,
            FactorCal = 100m,
            FilterLevel = 5,
            LoadCellFactors = new List<LoadCellFactorRecord>
            {
                new() { LoadCellId = 1, Factor = 1.5m },
                new() { LoadCellId = 2, Factor = 0.9m }
            }
        };

        _mockRepository.Setup(r => r.GetByNameAsync("default"))
            .ReturnsAsync(record);

        var result = await _service.LoadCalibrationAsync(_scale);

        Assert.True(result);
        Assert.Equal(4, _scale.NumberOfCells);
        
        var (zero, span, factor, filter) = _scale.GetCalibrationData();
        Assert.Equal(50000m, zero);
        Assert.Equal(150000m, span);
        Assert.Equal(100m, factor);
        Assert.Equal(5, filter);
        
        Assert.Equal(2, _scale.LoadCells.Count);
        Assert.Equal(1.5m, _scale.LoadCells.First(c => c.Id == 1).Factor);
    }

    [Fact]
    public async Task LoadCalibrationAsync_RecordNotFound_ReturnsFalse()
    {
        _mockRepository.Setup(r => r.GetByNameAsync("default"))
            .ReturnsAsync((CalibrationRecord?)null);

        var result = await _service.LoadCalibrationAsync(_scale);

        Assert.False(result);
    }

    [Fact]
    public async Task LoadCalibrationAsync_ConvertsGramsToKg()
    {
        var record = new CalibrationRecord
        {
            Name = "default",
            Unit = "g",
            CapMax = 10000m,
            Division = 1,
            DecimalPlaces = 0,
            ReferenceWeight = 5000m,
            ZeroConstant = 0m,
            SpanConstant = 100m,
            FactorCal = 100m,
            FilterLevel = 1
        };

        _mockRepository.Setup(r => r.GetByNameAsync("default"))
            .ReturnsAsync(record);

        await _service.LoadCalibrationAsync(_scale);

        Assert.Equal("g", _scale.Calibration.Unit);
        Assert.Equal(10000m, _scale.Calibration.CapMax);
    }

    [Fact]
    public async Task SaveCalibrationAsync_SavesAllData()
    {
        _scale.SetNumberOfCells(2);
        _scale.SetFilterLevel(3);
        _scale.UpdateLoadCell(1, 50000m);
        _scale.SetLoadCellFactor(1, 1.5m);
        _scale.CalibrateZero();
        _scale.UpdateLoadCell(1, 100000m);
        _scale.CalibrateSpan();

        CalibrationRecord? savedRecord = null;
        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<CalibrationRecord>()))
            .Callback<CalibrationRecord>(r => savedRecord = r)
            .ReturnsAsync((CalibrationRecord r) => r);

        await _service.SaveCalibrationAsync(_scale);

        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<CalibrationRecord>()), Times.Once);
        
        Assert.NotNull(savedRecord);
        Assert.Equal("default", savedRecord.Name);
        Assert.Equal(2, savedRecord.NumberOfCells);
        Assert.Equal(3, savedRecord.FilterLevel);
        Assert.Single(savedRecord.LoadCellFactors);
        Assert.Equal(1.5m, savedRecord.LoadCellFactors.First().Factor);
    }

    [Fact]
    public async Task SaveCalibrationAsync_SavesCalibrationConstants()
    {
        _scale.CalibrateZero();
        _scale.UpdateLoadCell(1, 100000m);
        _scale.CalibrateSpan();

        var (zero, span, factor, _) = _scale.GetCalibrationData();

        CalibrationRecord? savedRecord = null;
        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<CalibrationRecord>()))
            .Callback<CalibrationRecord>(r => savedRecord = r)
            .ReturnsAsync((CalibrationRecord r) => r);

        await _service.SaveCalibrationAsync(_scale);

        Assert.NotNull(savedRecord);
        Assert.Equal(zero, savedRecord.ZeroConstant);
        Assert.Equal(span, savedRecord.SpanConstant);
        Assert.Equal(factor, savedRecord.FactorCal);
    }

    [Fact]
    public async Task SaveCalibrationAsync_WithoutLoadCells_SavesEmptyFactors()
    {
        CalibrationRecord? savedRecord = null;
        _mockRepository.Setup(r => r.SaveAsync(It.IsAny<CalibrationRecord>()))
            .Callback<CalibrationRecord>(r => savedRecord = r)
            .ReturnsAsync((CalibrationRecord r) => r);

        await _service.SaveCalibrationAsync(_scale);

        Assert.NotNull(savedRecord);
        Assert.Empty(savedRecord.LoadCellFactors);
    }
}
