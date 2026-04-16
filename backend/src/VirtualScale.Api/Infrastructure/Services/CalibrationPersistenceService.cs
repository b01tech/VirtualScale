using VirtualScale.Api.Infrastructure.Persistence;
using VirtualScale.Api.Infrastructure.Persistence.Entities;
using VirtualScale.Api.Infrastructure.Repositories;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Infrastructure.Services;

public class CalibrationPersistenceService
{
    private readonly ICalibrationRepository _repository;
    private const string DefaultCalibrationName = "default";

    public CalibrationPersistenceService(ICalibrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> LoadCalibrationAsync(Scale scale)
    {
        var record = await _repository.GetByNameAsync(DefaultCalibrationName);
        if (record is null)
        {
            return false;
        }

        scale.SetNumberOfCells(record.NumberOfCells);
        scale.UpdateCalibrationSettings(
            record.NumberOfCells,
            record.Unit,
            record.Unit == "g" ? record.CapMax * 1000 : record.CapMax,
            record.Division,
            record.DecimalPlaces,
            record.Unit == "g" ? record.ReferenceWeight * 1000 : record.ReferenceWeight
        );

        var reflectionZero = typeof(Scale).GetProperty("ZeroConstant", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var reflectionSpan = typeof(Scale).GetProperty("SpanConstant",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var reflectionFactor = typeof(Scale).GetProperty("FactorCal",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var reflectionFilter = typeof(Scale).GetProperty("FilterLevel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        reflectionZero?.SetValue(scale, record.ZeroConstant);
        reflectionSpan?.SetValue(scale, record.SpanConstant);
        reflectionFactor?.SetValue(scale, record.FactorCal);
        reflectionFilter?.SetValue(scale, record.FilterLevel);

        foreach (var factor in record.LoadCellFactors)
        {
            scale.SetLoadCellFactor(factor.LoadCellId, factor.Factor);
        }

        return true;
    }

    public async Task SaveCalibrationAsync(Scale scale)
    {
        var record = new Persistence.Entities.CalibrationRecord
        {
            Name = DefaultCalibrationName,
            NumberOfCells = scale.NumberOfCells,
            Unit = scale.Calibration.Unit,
            CapMax = scale.Calibration.CapMax,
            Division = scale.Calibration.Division,
            DecimalPlaces = scale.Calibration.DecimalPlaces,
            ReferenceWeight = scale.Calibration.ReferenceWeight,
            ZeroConstant = GetPrivateField<decimal>(scale, "ZeroConstant"),
            SpanConstant = GetPrivateField<decimal>(scale, "SpanConstant"),
            FactorCal = GetPrivateField<decimal>(scale, "FactorCal"),
            FilterLevel = GetPrivateField<int>(scale, "FilterLevel"),
            LoadCellFactors = scale.LoadCells
                .Select(lc => new Persistence.Entities.LoadCellFactorRecord
                {
                    LoadCellId = lc.Id,
                    Factor = lc.Factor
                })
                .ToList()
        };

        await _repository.SaveAsync(record);
    }

    private static T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetProperty(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (T)(field?.GetValue(obj) ?? default!);
    }
}
