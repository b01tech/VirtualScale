namespace VirtualScale.Api.Infrastructure.Persistence.Entities;

public class LoadCellFactorRecord
{
    public int Id { get; set; }
    public int LoadCellId { get; set; }
    public decimal Factor { get; set; } = 1.0m;
    public int CalibrationRecordId { get; set; }
}
