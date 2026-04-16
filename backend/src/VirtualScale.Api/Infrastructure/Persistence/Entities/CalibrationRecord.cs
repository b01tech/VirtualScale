namespace VirtualScale.Api.Infrastructure.Persistence.Entities;

public class CalibrationRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = "default";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int NumberOfCells { get; set; } = 2;
    public string Unit { get; set; } = "kg";
    public decimal CapMax { get; set; } = 10m;
    public int Division { get; set; } = 2;
    public int DecimalPlaces { get; set; } = 3;
    public decimal ReferenceWeight { get; set; } = 10m;
    
    public decimal ZeroConstant { get; set; } = 0m;
    public decimal SpanConstant { get; set; } = 0m;
    public decimal FactorCal { get; set; } = 1000m;
    
    public int FilterLevel { get; set; } = 1;
    
    public List<LoadCellFactorRecord> LoadCellFactors { get; set; } = new();
}
