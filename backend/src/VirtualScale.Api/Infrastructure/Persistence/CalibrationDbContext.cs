using Microsoft.EntityFrameworkCore;

namespace VirtualScale.Api.Infrastructure.Persistence;

public class CalibrationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public CalibrationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<CalibrationDbContext> options) 
        : base(options)
    {
    }

    public Microsoft.EntityFrameworkCore.DbSet<Persistence.Entities.CalibrationRecord> CalibrationRecords => Set<Persistence.Entities.CalibrationRecord>();
    public Microsoft.EntityFrameworkCore.DbSet<Persistence.Entities.LoadCellFactorRecord> LoadCellFactors => Set<Persistence.Entities.LoadCellFactorRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Persistence.Entities.CalibrationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.Name).IsUnique();
            
            entity.HasMany(e => e.LoadCellFactors)
                .WithOne()
                .HasForeignKey(f => f.CalibrationRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Persistence.Entities.LoadCellFactorRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Factor).HasPrecision(18, 10);
        });
    }
}
