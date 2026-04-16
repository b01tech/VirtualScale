namespace VirtualScale.Api.Infrastructure.Repositories;

public interface ICalibrationRepository
{
    Task<Persistence.Entities.CalibrationRecord?> GetByNameAsync(string name);
    Task<Persistence.Entities.CalibrationRecord?> GetByIdAsync(int id);
    Task<List<Persistence.Entities.CalibrationRecord>> GetAllAsync();
    Task<Persistence.Entities.CalibrationRecord> SaveAsync(Persistence.Entities.CalibrationRecord record);
    Task DeleteAsync(int id);
}
