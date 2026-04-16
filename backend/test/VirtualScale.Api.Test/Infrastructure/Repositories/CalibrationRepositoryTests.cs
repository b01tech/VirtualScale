using Microsoft.EntityFrameworkCore;
using VirtualScale.Api.Infrastructure.Persistence;
using VirtualScale.Api.Infrastructure.Persistence.Entities;
using VirtualScale.Api.Infrastructure.Repositories;

namespace VirtualScale.Api.Test.Infrastructure.Repositories;

public class CalibrationRepositoryTests : IDisposable
{
    private readonly CalibrationDbContext _context;
    private readonly CalibrationRepository _repository;

    public CalibrationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CalibrationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new CalibrationDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        _repository = new CalibrationRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Fact]
    public async Task SaveAsync_NewRecord_CreatesRecord()
    {
        var record = new CalibrationRecord
        {
            Name = "test",
            NumberOfCells = 2,
            Unit = "kg",
            CapMax = 10m,
            Division = 2,
            DecimalPlaces = 3,
            ReferenceWeight = 5m,
            ZeroConstant = 50000m,
            SpanConstant = 150000m,
            FactorCal = 100m,
            FilterLevel = 3
        };

        var result = await _repository.SaveAsync(record);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public async Task SaveAsync_ExistingRecord_UpdatesRecord()
    {
        var record = new CalibrationRecord
        {
            Name = "update-test",
            NumberOfCells = 2,
            ZeroConstant = 50000m
        };
        await _repository.SaveAsync(record);

        record.ZeroConstant = 60000m;
        record.CapMax = 20m;
        await _repository.SaveAsync(record);

        var updated = await _repository.GetByNameAsync("update-test");
        
        Assert.Equal(60000m, updated!.ZeroConstant);
        Assert.Equal(20m, updated.CapMax);
    }

    [Fact]
    public async Task SaveAsync_WithLoadCellFactors_SavesFactors()
    {
        var record = new CalibrationRecord
        {
            Name = "with-factors",
            LoadCellFactors = new List<LoadCellFactorRecord>
            {
                new() { LoadCellId = 1, Factor = 1.5m },
                new() { LoadCellId = 2, Factor = 0.8m }
            }
        };

        await _repository.SaveAsync(record);

        var result = await _repository.GetByNameAsync("with-factors");
        
        Assert.Equal(2, result!.LoadCellFactors.Count);
    }

    [Fact]
    public async Task GetByNameAsync_ExistingRecord_ReturnsRecord()
    {
        var record = new CalibrationRecord { Name = "find-me" };
        await _repository.SaveAsync(record);

        var result = await _repository.GetByNameAsync("find-me");

        Assert.NotNull(result);
        Assert.Equal("find-me", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistingRecord_ReturnsNull()
    {
        var result = await _repository.GetByNameAsync("non-existent");
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRecords()
    {
        await _repository.SaveAsync(new CalibrationRecord { Name = "a" });
        await _repository.SaveAsync(new CalibrationRecord { Name = "b" });

        var results = await _repository.GetAllAsync();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task DeleteAsync_ExistingRecord_RemovesRecord()
    {
        var record = new CalibrationRecord { Name = "delete-me" };
        await _repository.SaveAsync(record);
        var id = record.Id;

        await _repository.DeleteAsync(id);

        var result = await _repository.GetByIdAsync(id);
        Assert.Null(result);
    }
}
