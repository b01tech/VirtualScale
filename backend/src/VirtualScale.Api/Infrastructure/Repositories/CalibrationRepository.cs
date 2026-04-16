using Microsoft.EntityFrameworkCore;
using VirtualScale.Api.Infrastructure.Persistence;

namespace VirtualScale.Api.Infrastructure.Repositories;

public class CalibrationRepository : ICalibrationRepository
{
    private readonly CalibrationDbContext _context;

    public CalibrationRepository(CalibrationDbContext context)
    {
        _context = context;
    }

    public async Task<Persistence.Entities.CalibrationRecord?> GetByNameAsync(string name)
    {
        return await _context
            .CalibrationRecords.Include(c => c.LoadCellFactors)
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Persistence.Entities.CalibrationRecord?> GetByIdAsync(int id)
    {
        return await _context.CalibrationRecords.Include(c => c.LoadCellFactors).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Persistence.Entities.CalibrationRecord>> GetAllAsync()
    {
        return await _context
            .CalibrationRecords.Include(c => c.LoadCellFactors)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Persistence.Entities.CalibrationRecord> SaveAsync(Persistence.Entities.CalibrationRecord record)
    {
        var existing = await _context
            .CalibrationRecords.Include(c => c.LoadCellFactors)
            .FirstOrDefaultAsync(c => c.Name == record.Name);

        if (existing is null)
        {
            record.CreatedAt = DateTime.UtcNow;
            record.UpdatedAt = DateTime.UtcNow;
            _context.CalibrationRecords.Add(record);
        }
        else
        {
            existing.UpdatedAt = DateTime.UtcNow;
            existing.NumberOfCells = record.NumberOfCells;
            existing.Unit = record.Unit;
            existing.CapMax = record.CapMax;
            existing.Division = record.Division;
            existing.DecimalPlaces = record.DecimalPlaces;
            existing.ReferenceWeight = record.ReferenceWeight;
            existing.ZeroConstant = record.ZeroConstant;
            existing.SpanConstant = record.SpanConstant;
            existing.FactorCal = record.FactorCal;
            existing.FilterLevel = record.FilterLevel;

            _context.LoadCellFactors.RemoveRange(existing.LoadCellFactors);
            foreach (var factor in record.LoadCellFactors)
            {
                factor.CalibrationRecordId = existing.Id;
                _context.LoadCellFactors.Add(factor);
            }

            record = existing;
        }

        await _context.SaveChangesAsync();
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await _context.CalibrationRecords.FindAsync(id);
        if (record is not null)
        {
            _context.CalibrationRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
