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

        scale.LoadCalibrationData(record.ZeroConstant, record.SpanConstant, record.FactorCal, record.FilterLevel);

        foreach (var factor in record.LoadCellFactors)
        {
            scale.SetLoadCellFactor(factor.LoadCellId, factor.Factor);
        }

        return true;
    }

    public async Task SaveCalibrationAsync(Scale scale)
    {
        if (scale.Calibration is null)
        {
            throw new InvalidOperationException("Scale calibration data is not initialized");
        }

        var calibration = scale.Calibration;
        var (zeroConstant, spanConstant, factorCal, filterLevel) = scale.GetCalibrationData();

        var record = new Persistence.Entities.CalibrationRecord
        {
            Name = DefaultCalibrationName,
            NumberOfCells = scale.NumberOfCells,
            Unit = calibration.Unit,
            CapMax = calibration.CapMax,
            Division = calibration.Division,
            DecimalPlaces = calibration.DecimalPlaces,
            ReferenceWeight = calibration.ReferenceWeight,
            ZeroConstant = zeroConstant,
            SpanConstant = spanConstant,
            FactorCal = factorCal,
            FilterLevel = filterLevel,
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
}
